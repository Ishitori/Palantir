namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API;

    public class VideoTrendChartBuilder : TrendChartBuilder
    {
        public VideoTrendChartBuilder(IMetricsService statisticsProvider) : base(statisticsProvider)
        {
        }

        protected override IEnumerable<PointInTime> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            return this.StatisticsProvider.GetVideos(projectId, dateRange, periodicity);
        }
    }
}