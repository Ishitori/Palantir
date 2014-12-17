namespace Ix.Palantir.Querying.Common
{
    using System;
    using System.Runtime.Serialization;

    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;

    [DataContract]
    public class DateRange
    {
        public DateRange(DateTime from, DateTime to, bool isSpecified = true)
        {
            this.From = from;
            this.To = to;
            this.IsSpecified = isSpecified;
        }

        public static DateRange Unspecified
        {
            get
            {
                return new DateRange(DateTimeHelper.GetEmptyDateTime(), DateTimeHelper.GetEmptyDateTime(), false);
            }
        }
        public static DateRange DayFromNow
        {
            get
            {
                DateTime from = BeginningOfCurrentHour.AddDays(-1);
                DateTime to = BeginningOfCurrentHour;

                return new DateRange(from, to);
            }
        }
        public static DateRange WeekFromNow
        {
            get
            {
                DateTime from = DateTimeHelper.GetDateTimeNow().Date.AddDays(-7);
                DateTime to = DateTimeHelper.GetDateTimeNow().Date;

                return new DateRange(from, to);
            }
        }
        public static DateRange MonthFromNow
        {
            get
            {
                DateTime from = DateTimeHelper.GetDateTimeNow().Date.AddMonths(-1);
                DateTime to = DateTimeHelper.GetDateTimeNow().Date;

                return new DateRange(from, to);
            }
        }
        public static DateRange YearFromNow
        {
            get
            {
                DateTime from = BeginningOfCurrentMonth.AddYears(-1);
                DateTime to = BeginningOfCurrentMonth;

                return new DateRange(from, to);
            }
        }

        [DataMember]
        public bool IsSpecified { get; set; }
        [DataMember]
        public DateTime From { get; set; }
        [DataMember]
        public DateTime To { get; set; }

        public int DaysInRange 
        {
            get
            {
                return (this.To - this.From).Days;
            }
        }

        private static DateTime BeginningOfCurrentMonth
        {
            get
            {
                return new DateTime(DateTimeHelper.GetDateTimeNow().Year, DateTimeHelper.GetDateTimeNow().Month, 1);
            }
        }
        private static DateTime BeginningOfCurrentHour
        {
            get
            {
                return new DateTime(DateTimeHelper.GetDateTimeNow().Year, DateTimeHelper.GetDateTimeNow().Month, DateTimeHelper.GetDateTimeNow().Day, DateTimeHelper.GetDateTimeNow().Hour, 0, 0);
            }
        }

        private static IDateTimeHelper DateTimeHelper
        {
            get
            {
                return Factory.GetInstance<IDateTimeHelper>();
            }
        }
    }
}