using System;

using iParkShared;

namespace iPark.Models
{
    public class Reservation : BaseModel
    {
        public string LotId { get; set; }
        public string PriceModelId { get; set; }
        public int Type { get; set; }
        public string CustomerRef { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public DateTimeOffset? ExpectedCheckin { get; set; }
        public DateTimeOffset? ExpectedCheckout { get; set; }
        public DateTimeOffset? ActualCheckin { get; set; }
        public DateTimeOffset? ActualCheckout { get; set; }
        public decimal? AdvancePaid { get; set; }
        public decimal? BalancePaid { get; set; }
        public string ConfNumAdvance { get; set; }
        public string ConfNumBalance { get; set; }

        public ParkingLot ParkingLot { get; set; }

        public PriceModel PriceModel { get; set; }

        public string ReservationTime
        {
            get
            {
                ReservationType rtype = ReservationType.GetType(Type);

                if (rtype == ReservationType.ST)
                {
                    DateTimeOffset? dto = (ParkingLot != null && ExpectedCheckin != null) ?
                                        ParkingLot.ConvertToLocalTime(ExpectedCheckin.Value) : ExpectedCheckin;
                    return $"{dto:d} {dto:t}";
                }
                else
                {
                    DateTimeOffset start = ParkingLot != null ? ParkingLot.ConvertToLocalTime(StartDate) : StartDate;
                    DateTimeOffset end = ParkingLot != null ? ParkingLot.ConvertToLocalTime(EndDate) : EndDate;

                    if(rtype == ReservationType.LTDaily || rtype == ReservationType.LTDailyWorkHrs)
                        return $"{start:d}";
                    else
                        return $"{start:d} - {end:d}";
                }
            }
        }
    }
}
