namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Converters;
    using Ix.Palantir.UI.Models.Chart.Builders.Trend;
    using Querying.Common;
    using Services.API;

    public abstract class TrendChartBuilder : TrendChartBuilderBase
    {
        protected TrendChartBuilder(IMetricsService statisticsProvider)
        {
            this.StatisticsProvider = statisticsProvider;
        }

        protected IMetricsService StatisticsProvider { get; private set; }

        public TrendChartData GetChart(int projectId, FilteringPeriod period)
        {
            DateRange dateRange = DateRangeConverter.GetDateRange(period);
            Periodicity periodicity = this.GetPeriodicity(period);
            IEnumerable<PointInTime> points = this.GetPoints(projectId, dateRange, periodicity);

            return new TrendChartData(points, period);
        }
        public TrendChartData GetChart(int projectId, DateRange dateRange)
        {
            var periodicity = dateRange.DaysInRange <= 1 ? Periodicity.ByHour : Periodicity.ByDay;
            var filteringPeriod = dateRange.DaysInRange <= 1 ? FilteringPeriod.Day : FilteringPeriod.Month;

            IEnumerable<PointInTime> points = this.GetPoints(projectId, dateRange, periodicity);

            return new TrendChartData(points, filteringPeriod);
        }

        protected abstract IEnumerable<PointInTime> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity);
    }
}