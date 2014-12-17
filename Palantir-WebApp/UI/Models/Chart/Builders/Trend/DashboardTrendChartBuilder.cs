namespace Ix.Palantir.UI.Models.Chart.Builders.Trend
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API;

    public class DashboardTrendChartBuilder : TrendMultiChartBuilder
    {
        public DashboardTrendChartBuilder(IMetricsService metricsService) : base(metricsService)
        {
        }

        protected override IEnumerable<IEnumerable<PointInTime>> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            return this.MetricsService.GetMetricData(projectId, dateRange, periodicity);
        }
    }
}