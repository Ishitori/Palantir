namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using DomainModel;
    using Ix.Palantir.Vkontakte.API.Access;

    public class GroupInfoProvider : IGroupInfoProvider
    {
        private const char CONST_SystemIdSeparator = '/';
        private readonly IVkConnectionBuilder connectionBuilder;

        public GroupInfoProvider(IVkConnectionBuilder connectionBuilder)
        {
            this.connectionBuilder = connectionBuilder;
        }

        public string GetVkGroupName(string groupUrl)
        {
            if (string.IsNullOrWhiteSpace(groupUrl))
            {
                return string.Empty;
            }

            var lastSlashIndex = groupUrl.LastIndexOf(CONST_SystemIdSeparator);

            if (lastSlashIndex == -1)
            {
                return string.Empty;
            }

            if (lastSlashIndex == groupUrl.Length - 1)
            {
                lastSlashIndex = groupUrl.LastIndexOf(CONST_SystemIdSeparator, groupUrl.Length - 2);
            }

            return groupUrl.Substring(lastSlashIndex).Trim(CONST_SystemIdSeparator);
        }
        public int GetVkGroupId(string groupUrl)
        {
            var vkDataProvider = this.connectionBuilder.GetVkDataProvider();
            var response = vkDataProvider.GetGroup(this.GetVkGroupName(groupUrl));
            return int.Parse(response.group[0].gid);
        }
        public VkGroupType GetVkGroupType(string groupUrl)
        {
            var vkDataProvider = this.connectionBuilder.GetVkDataProvider();
            var response = vkDataProvider.GetGroup(this.GetVkGroupName(groupUrl));

            switch (response.group[0].type)
            {
                case "page":
                    return VkGroupType.Page;

                case "event":
                    return VkGroupType.Event;

                case "group":
                    return VkGroupType.Group;

                default:
                    return VkGroupType.Page;
            }
        }
        public IList<long> GetVkGroupAdministratorIds(string groupUrl, VkGroupType groupType)
        {
            var vkDataProvider = this.connectionBuilder.GetVkDataProvider();
            var response = vkDataProvider.GetGroupAministrators(this.GetVkGroupName(groupUrl), (Ix.Palantir.Vkontakte.API.VkGroupType)(int)groupType);
            return response.AdminIds;
        }
    }
}