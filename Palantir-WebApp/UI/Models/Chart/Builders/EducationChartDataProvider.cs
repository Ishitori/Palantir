namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;

    public class EducationChartDataProvider
    {
        private readonly IMetricsService statisticsProvider;
        private readonly IMemberAdvancedSearchService searchService;

        public EducationChartDataProvider(IMetricsService statisticsProvider, IMemberAdvancedSearchService searchService = null)
        {
            this.statisticsProvider = statisticsProvider;
            this.searchService = searchService;
        }

        public PieChartData GetPieChartUsersData(int id, IList<ActiveUserInfo> userList)
        {
            return this.GetEducationPieChartData(this.searchService.GetEducationLevelInformationActiveUsers(id, userList));
        }

        public PieChartData GetPieChartUsersData(int id, long filterCode)
        {
            return this.GetEducationPieChartData(this.searchService.GetEducationLevelInformationActiveUsers(id, filterCode));
        }

        public PieChartData GetPieChartData(int id)
        {
            return this.GetEducationPieChartData(this.statisticsProvider.GetEducationLevelInformation(id));
        }

        private PieChartData GetEducationPieChartData(EducationLevelInformation data)
        {
            var pieChartData = new PieChartData();

            pieChartData.AddItem(new PieChartDataItem() { Label = "Не указано", Value = data.Unknown });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Среднее", Value = data.Middle });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Неоконченное высшее", Value = data.UncompletedHigher });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Высшее", Value = data.Higher });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Доктор наук", Value = data.PhD });

            return pieChartData;
        }
    }
}