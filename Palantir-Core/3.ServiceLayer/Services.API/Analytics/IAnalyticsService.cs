namespace Ix.Palantir.Services.API.Analytics
{
    using Ix.Palantir.Querying.Common;

    public interface IAnalyticsService
    {
        API.PostDensity GetPostMostCrowdedTime(int projectId, DateRange dateRange);
        UserStatistics GetInactiveUsersCount(int projectId, DateRange dateRange);
    }
}