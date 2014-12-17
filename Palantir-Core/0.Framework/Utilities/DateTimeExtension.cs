namespace Ix.Palantir.Utilities
{
    using System;
    using System.Globalization;

    public static class DateTimeExtension
    {
        public static double ToUnixTimestamp(this DateTime value)
        {
            // create Timespan by subtracting the value provided from the Unix Epoch
            TimeSpan span = value - UnixEpoh.Create();

            // return the total seconds (which is a UNIX timestamp)
            return span.TotalSeconds;
        }
        public static DateTime FromUnixTimestamp(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DateTime.MinValue;
            }

            double totalSeconds;

            if (!double.TryParse(value, out totalSeconds))
            {
                return DateTime.MinValue;
            }

            var timeSpan = TimeSpan.FromSeconds(totalSeconds);
            return UnixEpoh.Create().Add(timeSpan);
        }
        public static DateTime ToLocalUserDate(this DateTime utcDateTime, TimeZoneInfo localTimeZone)
        {
            TimeSpan utcOffset = localTimeZone.GetUtcOffset(utcDateTime);
            DateTimeOffset localDateTimeOffset = DateTime.SpecifyKind(utcDateTime.Add(utcOffset), DateTimeKind.Unspecified);

            return localDateTimeOffset.DateTime;
        }
        public static DateTime ToUtcDate(this DateTime localDateTime, TimeZoneInfo localTimeZone)
        {
            TimeSpan utcOffset = localTimeZone.GetUtcOffset(localDateTime);
            DateTimeOffset utcDateTimeOffset = DateTime.SpecifyKind(localDateTime.Add(-utcOffset), DateTimeKind.Unspecified);

            return utcDateTimeOffset.DateTime;
        }

        public static int GetWeekNumber(this DateTime value)
        {
            DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
            return currentInfo.Calendar.GetWeekOfYear(value, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
        }
    }
}