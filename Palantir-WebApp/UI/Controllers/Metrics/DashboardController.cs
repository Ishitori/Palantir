namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models;
    using Ix.Palantir.UI.Models.Chart.Builders.Trend;
    using Ix.Palantir.UI.Models.Converters;
    using Models.Chart;
    using Models.Metrics;
    using Querying.Common;
    using Querying.Common.DataFilters;
    using Services.API;
    using Services.API.Metrics;

    [ProjectOwnerSecurity]
    public class DashboardController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public DashboardController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            var result = this.metricsService.CheckAvailability(id);
            var checkAvailability = new CheckAvailabilityModel
            {
                IsReady = (bool)result[2],
                Done = (int)result[0],
                Total = (int)result[1]
            };
            var viewModel = new MetricsViewModel(metrics, checkAvailability);

            return this.MetricsView("Dashboard", viewModel);
        }

        /// <summary>
        /// Получить метрики.
        /// </summary>
        public ActionResult Metrics(int id, DataFilter filter)
        {
            DashboardMetrics metrics = this.metricsService.GetDashboardMetrics(id, DateRangeConverter.GetDateRange(filter));
            var viewModel = new DashboardViewModel(metrics);

            return this.MetricsView("_DashboardMetrics", viewModel);
        }

        /// <summary>
        /// Получить график.
        /// </summary>
        public ActionResult Chart(int id, DataFilter filter)
        {
            var trendChartBuilder = new DashboardTrendChartBuilder(this.metricsService);

            IEnumerable<TrendChartData> trendChartData = filter.Period != FilteringPeriod.Other
                ? trendChartBuilder.GetCharts(id, filter.Period)
                : trendChartBuilder.GetCharts(id, DateRangeConverter.GetDateRange(filter));
            List<TrendChartData> data = trendChartData.ToList();
            data[0].Name = "Динамика постов админов";
            data[0].ShowBars = true;
            data[0].ShowLines = false;
            data[0].ShowPoints = false;
            data[0].Stacking = true;
            data[0].BarWidth = 0.4;
            data[0].BarAlign = BarAlign.Center;
            
            data[1].Name = "Динамика постов пользователей";
            data[1].ShowBars = true;
            data[1].ShowLines = false;
            data[1].ShowPoints = false;
            data[1].BarOrder = 1;
            data[1].Stacking = true;
            data[1].BarWidth = 0.4;
            data[1].BarAlign = BarAlign.Center;

            data[2].Name = "Динамика общего количества постов";
            data[2].BarOrder = 2;
            data[2].FillLines = false;

            return this.Chart(trendChartData);
        }
    }
}
