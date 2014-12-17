namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API.Analytics;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Converters;
    using Ix.Palantir.UI.Models.Metrics;
    using Ix.Palantir.UI.Models.Shared;

    [ProjectOwnerSecurity]
    public class CompareGroupsController : MetricsController
    {
        private readonly IConcurrentAnalysisService analysisService;

        public CompareGroupsController(IConcurrentAnalysisService analysisService)
        {
            this.analysisService = analysisService;
        }

        public override ActionResult Index(int id)
        {
            var metrics = this.analysisService.GetConcurrentAnalysisIntro(id);
            var viewModel = new CompareGroupsInitViewModel(metrics);

            return this.MetricsView("CompareGroups", viewModel);
        }

        public ActionResult CompareGroups(int id, DataFilter filter, IList<int> concurrentIds)
        {
            this.analysisService.UpdateProjectsConcurrents(id, concurrentIds);
            var analysis = this.analysisService.GetConcurrentAnalysis(id, concurrentIds, DateRangeConverter.GetDateRange(filter));
            var converter = new UiTableCompareGroupModelConverter();
            IList<UiTableColumn> columnModel = converter.CreateCompareGroupsModel(analysis);
            return this.MetricsView("_GroupComparisonTable", columnModel);
        }
    }
}