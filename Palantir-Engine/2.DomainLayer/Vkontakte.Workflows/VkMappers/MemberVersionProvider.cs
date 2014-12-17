namespace Ix.Palantir.Vkontakte.Workflows.VkMappers
{
    using System.Collections.Generic;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class MemberVersionProvider : IMemberVersionProvider
    {
        private readonly IVkGroupRepository vkGroupRepository;
        private readonly IDictionary<int, int> versionCache;

        public MemberVersionProvider(IVkGroupRepository vkGroupRepository)
        {
            this.vkGroupRepository = vkGroupRepository;
            this.versionCache = new Dictionary<int, int>();
        }

        public int GetNextVersionNumber(int vkGroupId)
        {
            if (this.versionCache.ContainsKey(vkGroupId))
            {
                return this.versionCache[vkGroupId];
            }

            var state = this.vkGroupRepository.GetProcessingState(vkGroupId, DataFeedType.MembersInfo);
            var version = state != null ? state.Version + 1 : 1;

            this.versionCache[vkGroupId] = version;
            return version;
        }
    }
}