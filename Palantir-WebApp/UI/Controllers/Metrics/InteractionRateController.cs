namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Collections.Generic;
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
    public class InteractionRateController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public InteractionRateController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            var viewModel = new MetricsViewModel(metrics);

            return this.MetricsView("_InteractionRate", viewModel);
        }

        /// <summary>
        /// Получить график.
        /// </summary>
        public ActionResult Chart(int id, DataFilter filter)
        {
            var trendChartBuilder = new InteractionRateTrendChartBuilder(this.metricsService);
            DateRange dateRange = DateRangeConverter.GetDateRange(filter);
            
            IEnumerable<TrendChartData> trendChartData = filter.Period != FilteringPeriod.Other
                ? trendChartBuilder.GetCharts(id, filter.Period)
                : trendChartBuilder.GetCharts(id, dateRange);

            List<TrendChartData> data = trendChartData.ToList();
            data[0].Name = "IR";
            data[0].YaxisOrder = 2;

            data[1].ShowBars = true;
            data[1].ShowLines = false;
            data[1].ShowPoints = false;
            data[1].BarOrder = 1;
            data[1].BarWidth = 0.1;
            data[1].Name = "Комментарии";

            data[2].ShowBars = true;
            data[2].ShowLines = false;
            data[2].ShowPoints = false;
            data[2].BarOrder = 2;
            data[2].BarWidth = 0.1;
            data[2].Name = "Лайки";

            data[3].ShowBars = true;
            data[3].ShowLines = false;
            data[3].ShowPoints = false;
            data[3].BarOrder = 3;
            data[3].BarWidth = 0.1;
            data[3].Name = "Репосты";

            return this.Chart(trendChartData);
        }

        public ActionResult GetFrequencyChart(int id)
        {
            var data = this.metricsService.GetInteractionFrequency(id, 40);
            var graf1 = new TrendChartData();
            graf1.BarOrder = 1;
            graf1.Name = "Частота действия (F+)";
            data[0].ToList().ForEach(x => graf1.Values.Add(new ChartPoint_WithPercents(x.Item, x.Value, x.PercentsFromAllUsers, x.PerecentsFromActiveUsers)));

            var graf2 = new TrendChartData();
            graf2.BarOrder = 2;
            graf2.Name = "Строгая частота действия (F)";
            data[1].ToList().ForEach(x => graf2.Values.Add(new ChartPoint_WithPercents(x.Item, x.Value, x.PercentsFromAllUsers, x.PerecentsFromActiveUsers)));
            var result = new List<TrendChartData>() { graf1, graf2 };

            return this.Chart(result);
        }

        public ActionResult GetAverageCount(int id, DataFilter filter)
        {
            var data = this.metricsService.GetLikesCommentsRepostsAverageCount(id, DateRangeConverter.GetDateRange(filter)).ToList();
            var graf1 = new TrendChartData();
            graf1.BarOrder = 1;
            graf1.Name = "Лайки";
            graf1.ShowLabelInTip = true;
            graf1.Values.Add(new ChartPoint(data[0][0].Item, data[0][0].Value));
            graf1.Values.Add(new ChartPoint(data[0][1].Item, data[0][1].Value));
            graf1.Values.Add(new ChartPoint(data[0][2].Item, data[0][2].Value));

            var graf2 = new TrendChartData();
            graf2.BarOrder = 2;
            graf2.Name = "Комментарии";
            graf2.ShowLabelInTip = true;
            graf2.Values.Add(new ChartPoint(data[1][0].Item, data[1][0].Value));
            graf2.Values.Add(new ChartPoint(data[1][1].Item, data[1][1].Value));
            graf2.Values.Add(new ChartPoint(data[1][2].Item, data[1][2].Value));

            var graf3 = new TrendChartData();
            graf3.BarOrder = 3;
            graf3.Name = "Републикации";
            graf3.ShowLabelInTip = true;
            graf3.Values.Add(new ChartPoint(data[2][0].Item, data[2][0].Value));
            graf3.Values.Add(new ChartPoint(data[2][1].Item, data[2][1].Value));
            graf3.Values.Add(new ChartPoint(data[2][2].Item, data[2][2].Value));

            var result = new List<TrendChartData>() { graf1, graf2, graf3 };

            return this.Chart(result);
        }
    }
}
