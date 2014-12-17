namespace Ix.Palantir.DataAccess.API.StatisticsProviders
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;

    public interface IStatisticsProvider
    {
        IEnumerable<PointInTime> GetPosts(int projectId, DateRange range, Periodicity periodicity);
        IEnumerable<PointInTime> GetPhotos(int projectId, DateRange range, Periodicity periodicity);
        IEnumerable<PointInTime> GetVideos(int projectId, DateRange range, Periodicity periodicity);
        IEnumerable<IEnumerable<PointInTime>> GetUsersCount(int projectId, DateRange range, Periodicity periodicity);
        IEnumerable<IEnumerable<PointInTime>> GetDashboardDataChart(int projectId, DateRange range, Periodicity periodicity);
        CategorialValue GetEducationLevelInformation(int projectId);
        long GetAverageResponseTimeInTick(int projectId, DateRange range);

        DateTime? GetFirstPostDate(int projectId);
        DateTime? GetLastPostDate(int projectId);
        DateTime? GetFirstMemberDate(int projectId);
        DateTime? GetLastMemberDate(int projectId);
        IEnumerable<Post> GetMostPopularPosts(int projectId, PopularBy popularBy, DateRange dateRange, int postsCount = 30);
        
        int GetPhotosCount(int projectId, DateRange dateRange);
        int GetVideosCount(int projectId, DateRange dateRange);
        int GetPostsCount(int projectId, DateRange dateRange);
        int GetTopicsCount(int projectId, DateRange dateRange);
        int GetTopicCommentsCount(int projectId, DateRange dateRange);

        int[] GetPostReactionCount(int vkGroupId, DateRange dateRange);
        int[] GetPhotoReactionCount(int vkGroupId, DateRange dateRange);
        int[] GetVideoReactionCount(int vkGroupId, DateRange dateRange);

        IEnumerable<float> GetPostAverageCount(int projectId, DateRange dateRange);
        IEnumerable<float> GetPhotoAverageCount(int projectId, DateRange dateRange);
        IEnumerable<float> GetVideoAverageCount(int projectId, DateRange dateRange);

        IEnumerable<GroupedObject<long>> GetPostsCountByGroupAndDate(int projectId, DateRange dateRange);
        IEnumerable<GroupedObject<long>> GetPostsCommentCountByGroupAndDate(int projectId, DateRange dateRange);
        IEnumerable<GroupedObject<long>> GetTopicCommentCountByGroupAndDate(int projectId, DateRange dateRange);
        IEnumerable<GroupedObject<long>> GetVideoCommentCountByGroupAndDate(int projectId, DateRange dateRange);

        IEnumerable<ActiveUser> GetActiveUsers(int projectId, DateRange dateRange, int count = 40);

        CategorialValue GetGenderInformation(int projectId);
        CategorialValue GetAgeInformation(int projectId);
        IEnumerable<LocationInfoGroupedObject> GetMostPopularCities(int projectId, int cityCount = 30);

        IEnumerable<LocationInfoGroupedObject> GetMostPopularCitiesActiveUsers(int projectId, long[] usersId, int cityCount = 30);
        int GetMembersWithoutCityCount(int projectId);

        IList<City> GetCities(int projectId);
        IEnumerable<GroupedObject<string>> GetGroupInterests(int projectId, int interestsCount = 50);
        IEnumerable<NamedEntity> GetMembersByInterest(int projectId, string interestTitle);
        IList<MemberInterestsGroupedObject> GetMemberInterest(int projectId, long[] userIds, int? count = 50);
        IList<MemberInterestsGroupedObject> GetMemberInterest(int vkGroupId);

        IList<Photo> GetMostPopularPhotos(int projectId, PopularBy popularBy, DateRange dateRange, int count = 30);
        IList<Video> GetMostPopularVideos(int projectId, PopularBy popularBy, DateRange dateRange, int count = 30);
    }
}