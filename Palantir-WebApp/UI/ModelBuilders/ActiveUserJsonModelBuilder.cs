namespace Ix.Palantir.UI.ModelBuilders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Models.Chart;
    using Ix.Palantir.UI.Models.Chart.Builders;
    using Ix.Palantir.UI.Models.Converters;
    using Ix.Palantir.UI.Models.Metrics;
    using Ix.Palantir.UI.Renderers;

    public class ActiveUserJsonModelBuilder
    {
        private readonly IMetricsService metricsService;
        private readonly IMemberAdvancedSearchService searchService;

        public ActiveUserJsonModelBuilder(IMetricsService metricsService, IMemberAdvancedSearchService searchService)
        {
            this.metricsService = metricsService;
            this.searchService = searchService;
        }

        public ActiveUserJsonModel GetActiveUserModel(int id, DataFilter filter, ActiveUserJsonModelBuilderOptions options)
        {
            var result = new ActiveUserJsonModel();

            if (filter.Period == FilteringPeriod.Other && filter.DateRange.From > filter.DateRange.To)
            {
                return result;
            }

            var activeUsers = this.metricsService.GetUserMetrics(id, DateRangeConverter.GetDateRange(filter), options.UserTableCount);
            var usersList = activeUsers.MostActiveUsers.Take(options.ReportCount).ToList();
            var usersListIds = usersList.Select(u => u.Id).ToArray();
            var users = new ActiveUserViewModel(activeUsers);

            if (users.MostActiveUsers.Count != 0)
            {
                result.Table = this.GetTable(users.MostActiveUsers.ToList());
                result.InterestsData = this.GetInterests(id, usersListIds, options.InterestCount);
                result.AgeData = this.GetAgeInfo(id, usersList);
                result.GenderData = this.GetGenderInfo(id, usersList);
                result.EducationData = this.GetEducationInfo(id, usersList);
                result.CountryAndCityData = this.GetCountryAndCityInfo(id, usersListIds);
            }

            result.Table = this.GetTable(users.MostActiveUsers.ToList());
            return result;
        }

        private string GetTable(IEnumerable<MostActiveUsers> list)
        {
            var table = new UiTableRenderer();
            return table.RenderTable(list, "mostActiveUsers").ToHtmlString();
        }

        private string GetInterests(int projectId, long[] usersList, int interestCount)
        {
            var interests = this.metricsService.GetMemberInterests(projectId, usersList, interestCount).ToList();
            InterestsViewModel model = new InterestsViewModel(interests);
            return model.Interests;
        }

        private PieChartData GetAgeInfo(int projectId, IList<ActiveUserInfo> usersList)
        {
            var dataProvider = new AgeChartDataProvider(this.metricsService, this.searchService);
            return dataProvider.GetPieChartDataActiveUsers(projectId, usersList);
        }
        private PieChartData GetGenderInfo(int projectId, IList<ActiveUserInfo> usersList)
        {
            var dataProvider = new GenderChartDataProvider(this.metricsService, this.searchService);
            return dataProvider.GetPieChartDataActiveUsers(projectId, usersList);
        }
        private PieChartData GetEducationInfo(int projectId, IList<ActiveUserInfo> usersList)
        {
            var dataProvider = new EducationChartDataProvider(this.metricsService, this.searchService);
            return dataProvider.GetPieChartUsersData(projectId, usersList);
        }
        private string GetCountryAndCityInfo(int projectId, long[] usersListIds)
        {
            var cities = this.metricsService.GetMostPopularCities(projectId, usersListIds);
            var result = SocialViewModel.GetLocations(cities);
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(result);
        }
    }
}