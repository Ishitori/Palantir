namespace Ix.Palantir.DataAccess.API.StatisticsProviders
{
    using System.Collections.Generic;

    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public interface IRawDataProvider
    {
        IList<Post> GetPosts(int vkGroupId, DateRange dateRange);
        IList<Member> GetMembers(int vkGroupId);
        IDictionary<int, City> GetCities();
        IDictionary<int, Country> GetCountries();
        IList<Video> GetVideos(int vkGroupId, DateRange dateRange);
        IList<Photo> GetPhotos(int vkGroupId, DateRange dateRange);
        IList<MemberLike> GetMemberLikes(int vkGroupId);
        IList<MemberShare> GetMemberShares(int vkGroupId);
        IList<MemberStatExport> GetMemberStatInfo(int vkGroupId, string entityName, string groupKey);

        int GetMembersCount(int vkGroupId);

        IList<long> GetPostCreatorIds(int vkGroupId, DateRange dateRange);
        IList<long> GetPostCommentCreatorIds(int vkGroupId, DateRange dateRange);
        IList<long> GetTopicCreatorIds(int vkGroupId, DateRange dateRange);
        IList<long> GetTopicCommentCreatorIds(int vkGroupId, DateRange dateRange);
        IList<MemberWithStatus> GetUserIdsWithStatus(int vkGroupId);
    }
}