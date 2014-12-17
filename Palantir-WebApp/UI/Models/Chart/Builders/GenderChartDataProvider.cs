namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Services.API;
    using Services.API.Metrics;

    public class GenderChartDataProvider : IPieChartDataProvider
    {
        private readonly IMetricsService statisticsProvider;
        private readonly IMemberAdvancedSearchService searchService;

        public GenderChartDataProvider(IMetricsService statisticsProvider, IMemberAdvancedSearchService searchService = null)
        {
            this.statisticsProvider = statisticsProvider;
            this.searchService = searchService;
        }

        public PieChartData GetPieChartData(int projectId)
        {
            GenderInformation genderInformation = this.statisticsProvider.GetGenderInformation(projectId);

            return this.GetPieChartData(genderInformation);
        }

        public PieChartData GetPieChartDataActiveUsers(int projectId, long filterCode)
        {
            GenderInformation genderInformation = this.searchService.GetGenderInformationActiveUsers(projectId, filterCode);

            return this.GetPieChartData(genderInformation);
        }

        public PieChartData GetPieChartDataActiveUsers(int projectId, IList<ActiveUserInfo> userList)
        {
            GenderInformation genderInformation = this.searchService.GetGenderInformationActiveUsers(projectId, userList);

            return this.GetPieChartData(genderInformation);
        }

        private PieChartData GetPieChartData(GenderInformation genderInformation)
        {
            PieChartData pieChartData = new PieChartData();
            pieChartData.AddItem(new PieChartDataItem() { Label = "Мужчины", Value = genderInformation.Males });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Женщины", Value = genderInformation.Females });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Не указано", Value = genderInformation.Unknown });

            return pieChartData;
        }
    }
}