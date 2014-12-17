namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Linq;
    using System.Web.Mvc;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Converters;
    using Models.Chart;
    using Models.Chart.Builders;
    using Models.Metrics;
    using Querying.Common;
    using Querying.Common.DataFilters;
    using Services.API;
    using Services.API.Metrics;

    [ProjectOwnerSecurity]
    public class SocialController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public SocialController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            SocialMetrics metrics = this.metricsService.GetSocialMetrics(id);
            var viewModel = new SocialViewModel(metrics);

            return this.MetricsView("Social", viewModel);
        }

        public ActionResult SocialChart(int id, DataFilter filter)
        {
            var trendChartBuilder = new SocialTrendChartBuilder(this.metricsService);

            var trendChartData = filter.Period != FilteringPeriod.Other
                ? trendChartBuilder.GetCharts(id, filter.Period)
                : trendChartBuilder.GetCharts(id, DateRangeConverter.GetDateRange(filter));

            var data = trendChartData.ToList();

            data[0].Name = "Кол-во участников";
            data[0].YaxisOrder = 2;
            data[0].FillLines = false;

            /*data[1].ShowBars = true;
            data[1].ShowLines = false;
            data[1].ShowPoints = false;
            data[1].BarOrder = 1;
            data[1].Name = "Вступили";

            data[2].ShowBars = true;
            data[2].ShowLines = false;
            data[2].ShowPoints = false;
            data[2].BarOrder = 2;
            data[2].Name = "Покинули";*/

            return this.Chart(trendChartData);
        }

        public ActionResult GenderChart(int id)
        {
            IPieChartDataProvider dataProvider = new GenderChartDataProvider(this.metricsService);
            PieChartData pieChartData = dataProvider.GetPieChartData(id);

            return this.Chart(pieChartData);
        }

        public ActionResult AgeChart(int id)
        {
            IPieChartDataProvider dataProvider = new AgeChartDataProvider(this.metricsService);
            PieChartData pieChartData = dataProvider.GetPieChartData(id);

            return this.Chart(pieChartData);
        }

        public ActionResult EducationChart(int id)
        {
            var dataProvider = new EducationChartDataProvider(this.metricsService);

            return this.Chart(dataProvider.GetPieChartData(id));
        }
    }
}