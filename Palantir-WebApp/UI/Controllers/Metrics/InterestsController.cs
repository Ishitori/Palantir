namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Metrics;
    using Ix.Palantir.UI.Models.Shared;

    [ProjectOwnerSecurity]
    public class InterestsController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public InterestsController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            var viewModel = new MetricsViewModel(metrics);

            return this.MetricsView("Interests", viewModel);
        }

        public string GetUserInterestsTags(int id)
        {
            InterestsViewModel model = new InterestsViewModel(this.metricsService.GetMemberInterests(id, null, 40).ToList());
            return model.Interests;
        }

        [HttpGet]
        public ActionResult GetUserInterestsDetails(int id, string tagTitle)
        {
            var users = this.metricsService.GetUsersByInterest(id, tagTitle).OrderBy(x => x.Name);
            var list = new List<UserWithInterestModel>(
                users.Select(u => new UserWithInterestModel { UserLink = new UiLink { Text = u.Name, Url = u.Link, Target = "_blank" } }));

            return this.MetricsView("_SocialUserInterestsDetails", list);
        }
    }
}