namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Chart;
    using Ix.Palantir.UI.Models.Chart.Builders;
    using Ix.Palantir.UI.Models.Metrics;

    [ProjectOwnerSecurity]
    public class AudienceController : MetricsController
    {
        private readonly IMetricsService metricsService;
        private readonly IUserService userService;
        private readonly IMemberAdvancedSearchService searchService;

        public AudienceController(IMetricsService metricsService, IMemberAdvancedSearchService searchService, IUserService userService)
        {
            this.metricsService = metricsService;
            this.userService = userService;
            this.searchService = searchService;
        }

        public override ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            
            var cities = this.metricsService.GetCities(id).Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() }).ToList();
            cities.Insert(0, new SelectListItem());

            var filter = this.userService.GetUserAudienceFilter();
            var viewModel = new AudienceViewModel(metrics, cities, filter);
            
            return this.MetricsView("Audience", viewModel);
        }

        public JsonResult AudienceData(int id, AudienceFilter filter)
        {
            int membersCount = this.searchService.SearchAudience(id, filter);
            this.userService.SaveUserAudienceFilter(new JavaScriptSerializer().Serialize(filter));
            return this.Json(new AudienceSearchResult { Count = membersCount, FilterCode = filter.Code }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenderChart(int id, long filterCode)
        {
            var genderDataProvider = new GenderChartDataProvider(this.metricsService, this.searchService);
            var genderPieChartData = genderDataProvider.GetPieChartDataActiveUsers(id, filterCode);
            return this.Chart(genderPieChartData);
        }

        public ActionResult AgeChart(int id, long filterCode)
        {
            var ageDataProvider = new AgeChartDataProvider(this.metricsService, this.searchService);
            var agePieChartData = ageDataProvider.GetPieChartDataActiveUsersWithoutMonthAndDay(id, filterCode);
            return this.Chart(agePieChartData);
        }

        public ActionResult EducationChart(int id, long filterCode)
        {
            var educationDataProvider = new EducationChartDataProvider(this.metricsService, this.searchService);
            var education = educationDataProvider.GetPieChartUsersData(id, filterCode);
            return this.Chart(education);
        }

        public ActionResult CitiesMozaic(int id, long filterCode)
        {
            var cities = this.searchService.GetMostPopularCities(id, filterCode, count: 40).ToList();
            var result = SocialViewModel.GetLocations(cities);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InterestsMozaic(int id, long filterCode)
        {
            var interests = this.searchService.GetMemberInterests(id, filterCode, count: 40).ToList();
            InterestsViewModel model = new InterestsViewModel(interests);
            return this.Json(model.Object, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CLRChart(int id, long filterCode)
        {
            var dataProvider = new ActivityAnalysisDataProvider(this.searchService);
            var chart = dataProvider.GetLikesRepostCommentPieChartData(id, filterCode);
            return this.Chart(chart);
        }

        public ActionResult GetTypeOfContentChart(int id, long filterCode)
        {
            var dataProvider = new ActivityAnalysisDataProvider(this.searchService);
            PieChartData data = dataProvider.GetTypeOfContentPieChartData(id, filterCode);
            return this.Chart(data);
        }

        public PartialViewResult MemberSubTable(int id, long filterCode)
        {
            var data = this.searchService.MemberSubInfos(id, filterCode).OrderByDescending(x => x.Count).ToList();
            return this.PartialView("MemberSubList", data);
        }
    }
}
