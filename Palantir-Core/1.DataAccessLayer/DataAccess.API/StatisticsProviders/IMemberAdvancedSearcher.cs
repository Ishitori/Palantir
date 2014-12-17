namespace Ix.Palantir.DataAccess.API.StatisticsProviders
{
    using System.Collections.Generic;

    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;

    public interface IMemberAdvancedSearcher
    {
        IList<MemberMainInfo> GetAudienceByFilter(int projectId, AudienceFilter filter);

        CategorialValue GetAgeInformationActiveUsers(int vkGroupId, AudienceFilteringResult filteringResult, bool withoutMonthAndDay = false);

        CategorialValue GetGenderInformationActiveUsers(int vkGroupId, AudienceFilteringResult result);

        CategorialValue GetEducationLevelInformationActiveUsers(int vkGroupId, AudienceFilteringResult result);

        IEnumerable<GroupedObject<Country>> GetCountryInformationActiveUsers(int vkGroupId, AudienceFilteringResult result);

        IEnumerable<GroupedObject<City>> GetCityInformationActiveUsers(int vkGroupId, AudienceFilteringResult result);

        CategorialValue LikesRepostCommentDiagramData(int vkGroupId, AudienceFilteringResult result);

        CategorialValue GetTypeOfContentDiagramData(int vkGroupId, AudienceFilteringResult result);

        IList<MemberSubscriptionInfo> GetMemberSubscriptionInfo(int vkGroupId, AudienceFilteringResult result, int limit = 15);
    }
}