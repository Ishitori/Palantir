namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Web.Mvc;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.ModelBuilders;
    using Ix.Palantir.UI.Models.Metrics;

    [ProjectOwnerSecurity]
    public class ActiveUsersController : MetricsController
    {
        private readonly IMetricsService metricsService;
        private readonly IMemberAdvancedSearchService searchService;

        public ActiveUsersController(IMetricsService metricsService, IMemberAdvancedSearchService searchService)
        {
            this.metricsService = metricsService;
            this.searchService = searchService;
        }

        public override ActionResult Index(int id)
        {
            MetricsBase metricsBase = this.metricsService.GetBaseMetrics(id);
            var viewModel = new MetricsViewModel(metricsBase);
            
            return this.MetricsView("ActiveUsers", viewModel);
        }

        public ActionResult GetData(int id, DataFilter filter)
        {
            var builder = new ActiveUserJsonModelBuilder(this.metricsService, this.searchService);
            ActiveUserJsonModel json = builder.GetActiveUserModel(id, filter, ActiveUserJsonModelBuilderOptions.Default);
            return this.Chart(json);
        }
    }
}
