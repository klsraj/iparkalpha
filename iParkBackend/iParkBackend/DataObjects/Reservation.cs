using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace iParkBackend.DataObjects
{
    [Table("Reservation", Schema = "dbo")]
    public class Reservation : EntityData
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

        [ForeignKey("LotId")]
        public ParkingLot ParkingLot { get; set; }

        [ForeignKey("PriceModelId")]
        public PriceModel PriceModel { get; set; }
    }
}