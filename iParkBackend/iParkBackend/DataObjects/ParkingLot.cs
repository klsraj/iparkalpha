using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace iParkBackend.DataObjects
{
    [Table("ParkingLot", Schema = "dbo")]
    public class ParkingLot : EntityData
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Telephone { get; set; }
        public string Timezone { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int WorkHrsEnd { get; set; }
        public int Capacity { get; set; }
        public int Reserved { get; set; }

        [NotMapped]
        public double Distance { get; set; }

        [NotMapped]
        private TimeZoneInfo tziLocal = null;

        public TimeZoneInfo GetTimezone()
        {
            return tziLocal ?? (tziLocal = TimeZoneInfo.FromSerializedString(Timezone));
        }

        [NotMapped]
        public string WorkHrsEndText
        {
            get
            {
                int hour;
                string str;

                if (WorkHrsEnd < 12)
                {
                    hour = WorkHrsEnd;
                    str = "AM";
                }
                else if (WorkHrsEnd == 12)
                {
                    hour = WorkHrsEnd;
                    str = "Noon";
                }
                else
                {
                    hour = WorkHrsEnd - 12;
                    str = "PM";
                }

                return $"{hour} {str}";
            }
        }

        public DateTimeOffset ConvertToLocalTime(DateTimeOffset dto)
        {
            return TimeZoneInfo.ConvertTime(dto, GetTimezone());
        }

        public DateTime ConvertToLocalTime(DateTime dt, TimeZoneInfo tzi)
        {
            return TimeZoneInfo.ConvertTime(dt, tzi, GetTimezone());
        }
    }
}