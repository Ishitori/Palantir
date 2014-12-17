namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API;

    public interface IPieChartDataProvider
    {
        PieChartData GetPieChartData(int projectId);

        PieChartData GetPieChartDataActiveUsers(int projectId, IList<ActiveUserInfo> usersList);
    }
}