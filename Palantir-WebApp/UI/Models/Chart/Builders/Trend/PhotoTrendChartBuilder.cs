namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Querying.Common;
    using Services.API;

    public class PhotoTrendChartBuilder : TrendChartBuilder
    {
        public PhotoTrendChartBuilder(IMetricsService statisticsProvider) : base(statisticsProvider)
        {
        }

        protected override IEnumerable<PointInTime> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            return this.StatisticsProvider.GetPhotos(projectId, dateRange, periodicity);
        }
    }
}