using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

using iParkBackend.DataObjects;
using iParkBackend.Extensions;
using iParkBackend.Models;
using iParkShared;

namespace iParkBackend.Controllers
{
    [MobileAppController]
    public class ReservationSvcController : ApiController
    {
        [HttpGet]
        public List<ParkingLot> GetParkingList(double lat, double lng)
        {
            List<ParkingLot> parkingList = new List<ParkingLot>();

            string sql = $"SELECT p.*, dbo.fx_CalcDist(@lat, @lng, p.Latitude, p.Longitude) AS Distance FROM dbo.parkinglot p ORDER BY Distance";

            using (var context = new MobileServiceContext())
            {
                try
                {
                    // Get list of Parking Lots oredered by distance from the given position
                    // CollectionFromSql() is a custom Extension method of DbContext defined in Extensions.cs
                    // fx_CalcDist is a custom user defined database function to calculate distance
                    var mlist = context.CollectionFromSql(sql,
                        new Dictionary<string, object> {
                        { "@lat", lat },
                        { "@lng", lng },
                        });

                    foreach (IDictionary<string, Object> dict in mlist)
                    {
                        parkingList.Add(new ParkingLot
                        {
                            Id = (string)dict["Id"],
                            UpdatedAt = (DateTimeOffset)dict["UpdatedAt"],
                            CreatedAt = (DateTimeOffset)dict["CreatedAt"],
                            Name = (string)dict["Name"],
                            Street = (string)dict["Street"],
                            City = (string)dict["City"],
                            State = (string)dict["State"],
                            Zipcode = (string)dict["Zipcode"],
                            Telephone = (string)dict["Telephone"],
                            Timezone = (string)dict["Timezone"],
                            Latitude = (decimal)dict["Latitude"],
                            Longitude = (decimal)dict["Longitude"],
                            WorkHrsEnd = (int)dict["WorkHrsEnd"],
                            Capacity = (int)dict["Capacity"],
                            Reserved = (int)dict["Reserved"],
                            Distance = (double)dict["Distance"],
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw ServerUtils.BuildException(ex);
                }
            }

            return parkingList;
        }

        [HttpGet]
        public List<Reservation> GetReservations(string customerRef)
        {
            List<Reservation> reservations = new List<Reservation>();

            using (var context = new MobileServiceContext())
            {
                try
                {
                    // TODO - Logic for Timezone issues needs to be checked
                    DateTimeOffset today = DateTimeOffset.UtcNow;

                    reservations = context.Reservations
                                .Include(rr => rr.ParkingLot)
                                .Where(rr => rr.CustomerRef == customerRef &&
                                ((rr.Type == ReservationType.ST.Code && rr.EndDate >= today && rr.ActualCheckout == null) ||
                                (rr.Type != ReservationType.ST.Code && rr.EndDate >= today)))
                                .ToList();
                }
                catch (Exception ex)
                {
                    throw ServerUtils.BuildException(ex);
                }
            }

            return reservations;
        }

        [HttpPost]
        public PriceModel GetPriceModel(ParkingLot lot)
        {
            PriceModel priceModel = null;

            // Get current time in the Parking Lot Time zone
            DateTime now = lot.ConvertToLocalTime(DateTime.Now, TimeZoneInfo.Local);

            DateTime resDate = new DateTime(now.Year, now.Month, now.Day);
            int year = resDate.Year;
            int dayofWeek = (int)resDate.DayOfWeek;
            dayofWeek = dayofWeek == 0 ? 7 : dayofWeek; // Convert Sunday to 7
            int hour = now.Hour;

            string sql = $"SELECT * FROM dbo.pricemodel p WHERE p.LotId = '{lot.Id}'" +
                        $" AND DATEFROMPARTS({year}, p.StartMonth, p.StartDate) <= @resDate" +
                        $" AND DATEFROMPARTS({year}, p.EndMonth, p.EndDate) >= @resDate" +
                        $" AND p.StartDayofWeek <= {dayofWeek} AND p.EndDayofWeek >= {dayofWeek}" +
                        $" AND p.StartTime <= {hour} AND p.EndTime >= {hour}" +
                        $" ORDER BY DATEDIFF(d, DATEFROMPARTS({year}, p.StartMonth, p.StartDate), @resDate)" +
                        $", {dayofWeek} - p.StartDayofWeek, {hour} - p.StartTime";

            using (var context = new MobileServiceContext())
            {
                //priceModel = context.PriceModels.SqlQuery(sql, new SqlParameter("@resDate", resDate)).FirstOrDefault();
                priceModel = context.Database.SqlQuery<PriceModel>(sql, new SqlParameter("@resDate", resDate)).FirstOrDefault();
            }

            return priceModel;
        }

        [HttpPost]
        public string MakeReservation(Reservation resItem)
        {
            ReservationType rtype = ReservationType.GetType(resItem.Type);

            if (rtype == ReservationType.LTWeekly || rtype == ReservationType.LTWeeklyWorkHrs)
                resItem.EndDate = resItem.StartDate.AddDays(7);
            else if (rtype == ReservationType.LTMonthly || rtype == ReservationType.LTMonthlyWorkHrs)
                resItem.EndDate = resItem.StartDate.AddDays(30);
            else
                resItem.EndDate = resItem.StartDate.AddDays(1);

            resItem.EndDate = resItem.EndDate.AddSeconds(-1); // 12.59.59 PM Local time on the last day of reservation

            using (var context = new MobileServiceContext())
            {
                try
                {
                    ParkingLot parkingLot = context.ParkingLots.Find(resItem.LotId);

                    string sql = "SELECT COUNT(*) FROM Reservation rr WHERE rr.LotId = @lotid and @startDt <= rr.EndDate and @endDt >= rr.StartDate";

                    SqlParameter[] parms = new SqlParameter[]
                    {
                    new SqlParameter("@lotid", resItem.LotId),
                    new SqlParameter("@startDt", resItem.StartDate),
                    new SqlParameter("@endDt", resItem.EndDate),
                    };

                    int reserved = context.Database.SqlQuery<int>(sql, parms).FirstOrDefault();

                    if (reserved >= parkingLot.Capacity)
                        return "Parking not available for the selected date(s)";

                    PriceModel priceModel = context.PriceModels.Find(resItem.PriceModelId);
                    resItem.AdvancePaid = priceModel.GetAdvanceCharge(resItem.Type);

                    Utils.CreateStripeCharge(resItem.AdvancePaid.Value, parkingLot.Name, resItem.ConfNumAdvance);

                    parkingLot.Reserved++;
                    context.Reservations.Add(resItem);
                    context.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ServerUtils.BuildException(ex);
                }
            }

            return null;
        }

        [HttpPost]
        public string Checkin(string resCode)
        {
            using (var context = new MobileServiceContext())
            {
                Reservation rItem = context.Reservations
                                            .Include(rr => rr.ParkingLot)
                                            .Where(rr => rr.Id == resCode)
                                            .FirstOrDefault();

                if (rItem == null)
                    return $"Invalid Reservation code";

                ParkingLot lot = rItem.ParkingLot;

                if (rItem.ActualCheckin != null)
                    return String.Format("This code was already used to check in at {0:t} on {0:d}", lot.ConvertToLocalTime(rItem.ActualCheckin.Value));

                ReservationType rtype = ReservationType.GetType(rItem.Type);
                DateTime timeNow = DateTime.UtcNow;

                if (rtype == ReservationType.ST && timeNow > rItem.ExpectedCheckin.Value.AddMinutes(15))
                    return String.Format("This code is only valid until {0:t} on {0:d}", lot.ConvertToLocalTime(rItem.ExpectedCheckin.Value));

                if (rtype != ReservationType.ST && (timeNow < rItem.StartDate || timeNow > rItem.EndDate))
                {
                    if (rtype == ReservationType.LTDaily || rtype == ReservationType.LTDailyWorkHrs)
                        return $"This code is only valid on {lot.ConvertToLocalTime(rItem.StartDate):d}";
                    else
                        return $"This code is only valid from {lot.ConvertToLocalTime(rItem.StartDate):d} to {lot.ConvertToLocalTime(rItem.EndDate):d}";
                }

                if (rtype == ReservationType.LTDailyWorkHrs || rtype == ReservationType.LTWeeklyWorkHrs || rtype == ReservationType.LTMonthlyWorkHrs)
                {
                    DateTime timeNowLocal = lot.ConvertToLocalTime(timeNow, TimeZoneInfo.Utc);

                    if (timeNowLocal.Hour >= rItem.ParkingLot.WorkHrsEnd)
                        return $"This code is not valid after {lot.WorkHrsEndText}";

                    if (timeNowLocal.DayOfWeek == DayOfWeek.Saturday || timeNowLocal.DayOfWeek == DayOfWeek.Sunday)
                        return $"This code is not valid on Weekends";
                }

                rItem.ActualCheckin = timeNow;
                context.SaveChanges();
            }

            return null;
        }

        [HttpPost]
        public APIResponse Checkout(string resCode)
        {
            APIResponse resp = new APIResponse { Code = Constants.PROCESS_SUCCESS, Message = null, Result = null };

            using (var context = new MobileServiceContext())
            {
                try
                {
                    Reservation rItem = context.Reservations
                                                .Include(rr => rr.ParkingLot)
                                                .Include(rr => rr.PriceModel)
                                                .Where(rr => rr.Id == resCode)
                                                .FirstOrDefault();

                    string msg = VerifyCheckout(context, rItem);

                    if (msg != null)
                    {
                        resp.Code = Constants.PROCESS_ERROR;
                        resp.Message = msg;
                        return resp;
                    }

                    DateTimeOffset checkoutTime = DateTimeOffset.UtcNow;

                    decimal totalCharge = rItem.PriceModel.GetTotalCharge(rItem, checkoutTime);

                    if (totalCharge > rItem.AdvancePaid && rItem.BalancePaid == null)
                    {
                        decimal balance = totalCharge - rItem.AdvancePaid.Value;
                        resp.Code = Constants.BALANCE_DUE;
                        resp.Message = $"Your balance is: {balance:C2}";
                        resp.Result = "<" + checkoutTime.ToString("O") + ">";

                        return resp;
                    }

                    FinalizeCheckout(context, rItem, checkoutTime);
                }
                catch (Exception ex)
                {
                    throw ServerUtils.BuildException(ex);
                }
            }

            return resp;
        }

        [HttpPost]
        public string PayBalance(string resCode, string confNum, string checkout)
        {
            using (var context = new MobileServiceContext())
            {
                try
                {
                    Reservation rItem = context.Reservations
                                                .Include(rr => rr.ParkingLot)
                                                .Include(rr => rr.PriceModel)
                                                .Where(rr => rr.Id == resCode)
                                                .FirstOrDefault();

                    // Convert serialized string received from Client into checkout time
                    // The string is serialized in Checkout() method above and sent to the Client
                    // The serialized string is delimited by "<" and ">" at the beginning and end
                    DateTimeOffset checkoutTime = DateTimeOffset.Parse(checkout.Substring(1, checkout.Length - 2),
                                                                                    null, DateTimeStyles.RoundtripKind);

                    decimal balance = rItem.PriceModel.GetTotalCharge(rItem, checkoutTime) - rItem.AdvancePaid.Value;

                    Utils.CreateStripeCharge(balance, rItem.ParkingLot.Name, confNum);

                    rItem.BalancePaid = balance;
                    rItem.ConfNumBalance = confNum;

                    FinalizeCheckout(context, rItem, checkoutTime);
                }
                catch (Exception ex)
                {
                    throw ServerUtils.BuildException(ex);
                }
            }

            return null;
        }

        private string VerifyCheckout(MobileServiceContext context, Reservation rItem)
        {
            if (rItem == null)
                return $"Invalid Reservation code";

            if (rItem.ActualCheckin == null)
                return $"This code is not yet used to check in";

            if (rItem.ActualCheckout != null)
                return String.Format("This code was already used to check out at {0:t} on {0:d}", rItem.ParkingLot.ConvertToLocalTime(rItem.ActualCheckout.Value));

            return null;
        }

        private void FinalizeCheckout(MobileServiceContext context, Reservation rItem, DateTimeOffset checkoutTime)
        {
            ReservationType rtype = ReservationType.GetType(rItem.Type);

            if (rtype == ReservationType.ST)
                rItem.ActualCheckout = checkoutTime;
            else
                rItem.ActualCheckin = rItem.ActualCheckout = null;

            context.SaveChanges();
        }
    }
}

