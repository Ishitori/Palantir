namespace Ix.Palantir.UI.Models.Chart.Builders.Trend
{
    using System;
    using Ix.Palantir.Querying.Common;

    public abstract class TrendChartBuilderBase
    {
        protected Periodicity GetPeriodicity(FilteringPeriod period)
        {
            switch (period)
            {
                case FilteringPeriod.Day:
                    return Periodicity.ByHour;

                case FilteringPeriod.Week:
                case FilteringPeriod.Month:
                    return Periodicity.ByDay;

                case FilteringPeriod.Year:
                    return Periodicity.ByMonth;
            }

            throw new ArgumentException("Unexpected ChartPeriod value", "period");
        }
    }
}