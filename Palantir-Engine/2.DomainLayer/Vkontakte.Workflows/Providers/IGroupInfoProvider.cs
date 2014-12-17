namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using DomainModel;

    public interface IGroupInfoProvider
    {
        string GetVkGroupName(string groupUrl);
        int GetVkGroupId(string groupUrl);
        IList<long> GetVkGroupAdministratorIds(string groupUrl, VkGroupType groupType);
        VkGroupType GetVkGroupType(string groupUrl);
    }
}