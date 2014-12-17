namespace Ix.Palantir.UI.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Settings;

    [ProjectOwnerSecurity]
    public class SettingsController : Controller
    {
        private readonly IMetricsService metricsService;
        private readonly ISettingsService settingsService;
        private readonly ICurrentUserProvider currentUserProvider;

        public SettingsController(IMetricsService metricsService, ISettingsService settingsService, ICurrentUserProvider currentUserProvider)
        {
            this.metricsService = metricsService;
            this.settingsService = settingsService;
            this.currentUserProvider = currentUserProvider;
        }

        public ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            IList<GroupProcessingItem> history = this.settingsService.GetProcessingHistory(id);
            var canDeleteProjects = this.currentUserProvider.GetAccountOfCurrentUser().CanDeleteProjects;
            GroupSettingsViewModel viewModel = new GroupSettingsViewModel(metrics, history, canDeleteProjects);

            return this.View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            this.settingsService.DeleteProject(id);
            return this.RedirectToAction("Index", "Home");
        }
    }
}