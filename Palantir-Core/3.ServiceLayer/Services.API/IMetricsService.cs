namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API.Metrics;

    public interface IMetricsService
    {
        DashboardMetrics GetDashboardMetrics(int projectId, DateRange dateRange);
        PostsMetrics GetPostsMetrics(int projectId);
        
        IEnumerable<PointInTime> GetPhotos(int projectId, DateRange dateRange, Periodicity periodicity);
        IEnumerable<IEnumerable<PointInTime>> GetUsersCount(int projectId, DateRange dateRange, Periodicity periodicity);
        IEnumerable<PointInTime> GetPosts(int projectId, DateRange dateRange, Periodicity periodicity);
        IEnumerable<PointInTime> GetVideos(int projectId, DateRange dateRange, Periodicity periodicity);
        IEnumerable<IEnumerable<PointInTime>> GetInteractionRate(int projectId, DateRange range, Periodicity periodicity);
        IList<IList<GroupedObjectWithPercents<string>>> GetInteractionFrequency(int projectId, int count);
        IEnumerable<IEnumerable<PointInTime>> GetMetricData(int projectId, DateRange range, Periodicity periodicity);
        EducationLevelInformation GetEducationLevelInformation(int projectid);
        DateRange PostDateLimit(int projectId);
        DateRange MemberDateLimit(int projectId);

        object[] CheckAvailability(int projectId);

        IList<IList<GroupedObjectDouble<string>>> GetLikesCommentsRepostsAverageCount(int projectId, DateRange range);

        GenderInformation GetGenderInformation(int projectId);
        AgeInformation GetAgeInformation(int projectId);
        SocialMetrics GetSocialMetrics(int projectId);
        IEnumerable<VkMemberInterest> GetMemberInterests(int projectId, int interestsCount = 50);
        IEnumerable<VkMemberInfo> GetUsersByInterest(int projectId, string interestTitle);

        UserMetrics GetUserMetrics(int projectId, DateRange dateRange, int count);
        IDictionary<int, string> GetCities(int projectId);
 
        IList<PopularCityInfo> GetMostPopularCities(int projectId, long[] usersIds);

        /// <summary>
        /// Получить общую информацию по метрикам.
        /// </summary>
        Metrics.MetricsBase GetBaseMetrics(int projectId);

        IList<PostEntityInfo> GetMostPopularPosts(int projectId, DateRange dateRange);
        IList<ContentEntityInfo> GetMostPopularContent(int projectId, DateRange dateRange);

        IList<MemberInterestsObject> GetMemberInterests(int projectId, long[] usersId, int count);
    }
}