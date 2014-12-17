namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Querying.Common;
    using Services.API;

    public class PostTrendChartBuilder : TrendChartBuilder
    {
        public PostTrendChartBuilder(IMetricsService statisticsProvider)
            : base(statisticsProvider)
        {
        }

        protected override IEnumerable<PointInTime> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            return this.StatisticsProvider.GetPosts(projectId, dateRange, periodicity);
        }
    }
}