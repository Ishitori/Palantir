namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Services.API;
    using Services.API.Metrics;

    public class AgeChartDataProvider : IPieChartDataProvider
    {
        private readonly IMetricsService statisticsProvider;
        private readonly IMemberAdvancedSearchService searchService;

        public AgeChartDataProvider(IMetricsService statisticsProvider, IMemberAdvancedSearchService searchService = null)
        {
            this.statisticsProvider = statisticsProvider;
            this.searchService = searchService;
        }

        public PieChartData GetPieChartData(int projectId)
        {
            AgeInformation genderInformation = this.statisticsProvider.GetAgeInformation(projectId);
            return this.GetPieChartData(genderInformation);
        }

        public PieChartData GetPieChartDataActiveUsers(int projectId, IList<ActiveUserInfo> usersList)
        {
            var ageActiveUsers = this.searchService.GetAgeActiveUsers(projectId, usersList, false);
            AgeInformation genderInformation = AgeInformation.Create(ageActiveUsers);

            return this.GetPieChartData(genderInformation);
        }

        public PieChartData GetPieChartDataActiveUsersWithoutMonthAndDay(int projectId, long filterCode)
        {
            var ageActiveUsers = this.searchService.GetAgeActiveUsers(projectId, filterCode, true);
            AgeInformation genderInformation = AgeInformation.Create(ageActiveUsers);

            return this.GetPieChartData(genderInformation);
        }
        private PieChartData GetPieChartData(AgeInformation ageInformation)
        {
            PieChartData pieChartData = new PieChartData();
            pieChartData.AddItem(new PieChartDataItem() { Label = "< 14 лет", Value = ageInformation.Below14 });
            pieChartData.AddItem(new PieChartDataItem() { Label = "15-24 года", Value = ageInformation.Upper15Below24 });
            pieChartData.AddItem(new PieChartDataItem() { Label = "25-34 года", Value = ageInformation.Upper25Below34 });
            pieChartData.AddItem(new PieChartDataItem() { Label = "35-44 года", Value = ageInformation.Upper35Below44 });
            pieChartData.AddItem(new PieChartDataItem() { Label = "45-54 года", Value = ageInformation.Upper45Below54 });
            pieChartData.AddItem(new PieChartDataItem() { Label = "> 55 лет", Value = ageInformation.Upper55 });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Не указано", Value = ageInformation.Unknown });

            return pieChartData;
        }
    }
}