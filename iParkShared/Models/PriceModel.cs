
namespace iPark.Models
{
    public class PriceModel : BaseModel
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

        public string MinTimeText
        {
            get
            {
                string str = "hr";

                switch(MinTimeUnit.ToUpper())
                {
                    case "H": str = "hr"; break;
                    case "D": str = "day"; break;
                    case "W": str = "week"; break;
                    case "M": str = "month"; break;
                }

                return MinTime.ToString() + " " + (MinTime > 1 ? str + "s" : str);
            }
        }

        public decimal Rate
        {
            get
            {
                decimal? rt = HourlyRate;

                switch (MinTimeUnit.ToUpper())
                {
                    case "H": rt = HourlyRate; break;
                    case "D": rt = DailyRate; break;
                    case "W": rt = WeeklyRate; break;
                    case "M": rt = MonthlyRate; break;
                }

                return (decimal)rt;
            }
        }

        public string RateText
        {
            get
            {
                string str = "/hr";

                switch (MinTimeUnit.ToUpper())
                {
                    case "H": str = "/hr"; break;
                    case "D": str = "/day"; break;
                    case "W": str = "/week"; break;
                    case "M": str = "/month"; break;
                }

                return $"{Rate:C2} {str}";
            }
        }
    }
}
