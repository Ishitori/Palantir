namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Web.Mvc;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Metrics;

    [ProjectOwnerSecurity]
    public class AttitudeController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public AttitudeController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            var metrics = this.metricsService.GetBaseMetrics(id);
            MetricsViewModel viewModel = new MetricsViewModel(metrics);

            return this.MetricsView("Attitude", viewModel);
        }
    }
}