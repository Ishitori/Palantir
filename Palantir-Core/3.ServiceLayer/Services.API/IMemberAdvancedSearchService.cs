namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API.Metrics;

    public interface IMemberAdvancedSearchService
    {
        int SearchAudience(int projectId, AudienceFilter filter);

        CategorialValue GetAgeActiveUsers(int projectId, long filterCode, bool withoutMonthAndDay = false);

        GenderInformation GetGenderInformationActiveUsers(int projectId, long filterCode);

        EducationLevelInformation GetEducationLevelInformationActiveUsers(int projectId, long filterCode);

        LRCDiagramDataInfo GetLikesRepostCommentDiagramData(int projectId, long filterCode);

        TypeOfContentDataInfo GetTypeOfContentDiagramData(int projectId, long filterCode);

        IList<MemberSubInfo> MemberSubInfos(int projectId, long filterCode, int limit = 15);

        CategorialValue GetAgeActiveUsers(int projectId, IList<ActiveUserInfo> users, bool withoutMonthAndDay = false);

        GenderInformation GetGenderInformationActiveUsers(int projectId, IList<ActiveUserInfo> users);

        EducationLevelInformation GetEducationLevelInformationActiveUsers(int projectId, IList<ActiveUserInfo> users);

        IEnumerable<PopularCityInfo> GetMostPopularCities(int projectId, long filterCode, int count);

        IList<MemberInterestsObject> GetMemberInterests(int projectId, long filterCode, int count);
    }
}