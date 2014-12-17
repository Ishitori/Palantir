namespace Ix.Palantir.UI.Models.Converters
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;

    public static class DateRangeConverter
    {
        public static DateRange GetDateRange(FilteringPeriod period)
        {
            DateRange temp;
            
            switch (period)
            {
                case FilteringPeriod.Day:
                    temp = DateRange.DayFromNow;
                    temp.To = temp.To.AddHours(1);
                    return temp;

                case FilteringPeriod.Week:
                    temp = DateRange.WeekFromNow;
                    temp.To = temp.To.AddDays(1);
                    return temp;

                case FilteringPeriod.Month:
                    temp = DateRange.MonthFromNow;
                    temp.To = temp.To.AddDays(1);
                    return temp;

                case FilteringPeriod.Year:
                    temp = DateRange.YearFromNow;
                    temp.To = temp.To.AddMonths(1);
                    return temp;
            }

            throw new PalantirException("Unexpected period value");
        }

        public static DateRange GetDateRange(DataFilter filter)
        {
            if (filter.Period != FilteringPeriod.Other)
            {
                return GetDateRange(filter.Period);
            }

            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();
            DateTime from = dateTimeHelper.GetUtcDate(filter.DateRange.From);
            DateTime to = dateTimeHelper.GetUtcDate(filter.DateRange.To);
            var result = new DateRange(from, to.AddDays(1)); // include last day in search
            return result;
        }
    }
}