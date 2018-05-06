using System;
using System.Collections.Generic;

namespace iParkShared
{
    public class ReservationType
    {
        public static readonly ReservationType ST = new ReservationType(0, "Short Term");
        public static readonly ReservationType LTDaily = new ReservationType(1, "Daily");
        public static readonly ReservationType LTDailyWorkHrs = new ReservationType(2, "Daily (Work Hrs)");
        public static readonly ReservationType LTWeekly = new ReservationType(3, "Weekly");
        public static readonly ReservationType LTWeeklyWorkHrs = new ReservationType(4, "Weekly (Work Hrs)");
        public static readonly ReservationType LTMonthly = new ReservationType(5, "Monthly");
        public static readonly ReservationType LTMonthlyWorkHrs = new ReservationType(6, "Monthly (Work Hrs)");

        public static readonly List<ReservationType> LongTerm =
            new List<ReservationType> { LTDaily, LTDailyWorkHrs, LTWeekly, LTWeeklyWorkHrs, LTMonthly, LTMonthlyWorkHrs };

        public ReservationType(int code, string name)
        {
            Code = code;
            Name = name;
        }

        public int Code { get; set; }
        public string Name { get; set; }

        public static ReservationType GetType(int code)
        {
            if (code == ST.Code)
                return ST;
            if (code == LTDaily.Code)
                return LTDaily;
            if (code == LTDailyWorkHrs.Code)
                return LTDailyWorkHrs;
            if (code == LTWeekly.Code)
                return LTWeekly;
            if (code == LTWeeklyWorkHrs.Code)
                return LTWeeklyWorkHrs;
            if (code == LTMonthly.Code)
                return LTMonthly;
            if (code == LTMonthlyWorkHrs.Code)
                return LTMonthlyWorkHrs;

            throw new Exception("ReservationType.GetType : Invalid Code - " + code);
        }
    }
}
