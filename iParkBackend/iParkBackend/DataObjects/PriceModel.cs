using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

using iParkShared;

namespace iParkBackend.DataObjects
{
    [Table("PriceModel", Schema = "dbo")]
    public class PriceModel : EntityData
    {
        public string LotId { get; set; }
        public int StartMonth { get; set; }
        public int StartDate { get; set; }
        public int EndMonth { get; set; }
        public int EndDate { get; set; }
        public int StartDayofWeek { get; set; }
        public int EndDayofWeek { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? DailyRate { get; set; }
        public decimal? DailyRateWorkHrs { get; set; }
        public decimal? WeeklyRate { get; set; }
        public decimal? WeeklyRateWorkHrs { get; set; }
        public decimal? MonthlyRate { get; set; }
        public decimal? MonthlyRateWorkHrs { get; set; }
        public decimal? MinTime { get; set; }
        public string MinTimeUnit { get; set; }
        public decimal? Increment { get; set; }
        public decimal? AdvanceTime { get; set; }

        public decimal GetMinimumCharge()
        {
            decimal? rate = HourlyRate;

            switch(MinTimeUnit.ToUpper())
            {
                case "H": rate = HourlyRate; break;
                case "D": rate = DailyRate; break;
                case "W": rate = WeeklyRate; break;
                case "M": rate = MonthlyRate; break;
            }

            return (decimal)(MinTime == null ? rate : MinTime * rate);
        }

        public decimal GetAdvanceCharge(int type)
        {
            decimal? rt;
            ReservationType rtype = ReservationType.GetType(type);

            if (rtype == ReservationType.ST)
                rt = GetMinimumCharge();
            else
                rt = GetRateforType(rtype);

            return rt.Value;
        }

        public decimal GetTotalCharge(Reservation rItem, DateTimeOffset checkoutTime)
        {
            ReservationType rtype = ReservationType.GetType(rItem.Type);

            decimal charge = GetAdvanceCharge(rItem.Type);

            if (rtype == ReservationType.ST)
            {
                decimal totCharge = GetChargeforTime(rItem.ActualCheckin.Value, checkoutTime);
                if (totCharge > charge)
                    charge = totCharge;
            }
            else
            {
                if (checkoutTime > rItem.EndDate)
                    charge += GetChargeforTime(rItem.EndDate, checkoutTime);
            }

            return charge;
        }

        private decimal GetRateforType(ReservationType rtype)
        {
            decimal? rt;

            if (rtype == ReservationType.ST)
                rt = HourlyRate;
            else if (rtype == ReservationType.LTDaily)
                rt = DailyRate;
            else if (rtype == ReservationType.LTDailyWorkHrs)
                rt = DailyRateWorkHrs;
            else if (rtype == ReservationType.LTWeekly)
                rt = WeeklyRate;
            else if (rtype == ReservationType.LTWeeklyWorkHrs)
                rt = WeeklyRateWorkHrs;
            else if (rtype == ReservationType.LTMonthly)
                rt = MonthlyRate;
            else if (rtype == ReservationType.LTMonthlyWorkHrs)
                rt = MonthlyRateWorkHrs;
            else
                throw new Exception($"PriceModel.GetRateforType : Invalid Type - {rtype.Code} - {rtype.Name}");

            return rt.Value;
        }

        private decimal GetChargeforTime(DateTimeOffset start, DateTimeOffset end)
        {
            TimeSpan tdiff = end - start;
            int days, weeks, months;
            decimal hours;

            days = tdiff.Days;
            months = days / 30;
            days -= months * 30;
            weeks = days / 7;
            days -= weeks * 7;

            hours = tdiff.Hours;
            if (tdiff.Minutes > 45)
                hours += 1;
            else if (tdiff.Minutes > 30)
                hours += (decimal)0.45;
            else if (tdiff.Minutes > 15)
                hours += (decimal)0.5;
            else if (tdiff.Minutes > 0)
                hours += (decimal)0.25;

            decimal? rt = months * MonthlyRate + weeks * WeeklyRate + days * DailyRate + hours * HourlyRate;
            return rt.Value;
        }
    }
}