namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Linq;
    using System.Web.Mvc;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Chart;
    using Ix.Palantir.UI.Models.Chart.Builders;
    using Ix.Palantir.UI.Models.Converters;
    using Ix.Palantir.UI.Models.Metrics;

    [ProjectOwnerSecurity]
    public class ContentController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public ContentController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            MetricsViewModel viewModel = new MetricsViewModel(metrics);

            return this.MetricsView("Content", viewModel);
        }

        public ActionResult Chart(int id, DataFilter filter)
        {
            var trendChartBuilder = new PhotoTrendChartBuilder(this.metricsService);
            DateRange dateRange = DateRangeConverter.GetDateRange(filter);

            TrendChartData trendChartData = filter.Period != FilteringPeriod.Other
                ? trendChartBuilder.GetChart(id, filter.Period)
                : trendChartBuilder.GetChart(id, dateRange);

            var trendChartBuilder2 = new VideoTrendChartBuilder(this.metricsService);
            TrendChartData trendChartData2 = filter.Period != FilteringPeriod.Other
                ? trendChartBuilder2.GetChart(id, filter.Period)
                : trendChartBuilder2.GetChart(id, dateRange);

            trendChartData2.YaxisOrder = 3;
            trendChartData.Name = "Фото";
            trendChartData2.Name = "Видео";

            return this.Chart(new[] { trendChartData, trendChartData2 });
        }

        public ActionResult GetMostPopularContents(int id, DataFilter filter)
        {
            var converter = new UiTableModelsConverter();
            var contentEntities = this.metricsService.GetMostPopularContent(id, DateRangeConverter.GetDateRange(filter));
            return this.MetricsView("_MostPopularContent", contentEntities.Select(converter.CreateContentModel).ToList());
        }        
    }
}