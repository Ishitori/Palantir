namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using Ix.Palantir.Configuration;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class ProvidingStrategy : IProvidingStrategy
    {
        private readonly IConfigurationProvider configurationProvider;
        private readonly IVkGroupRepository vkGroupRepository;

        public ProvidingStrategy(IConfigurationProvider configurationProvider, IVkGroupRepository vkGroupRepository)
        {
            this.configurationProvider = configurationProvider;
            this.vkGroupRepository = vkGroupRepository;
        }

        public bool IsLimitedProcessingEnabled(int vkGroupId, DataFeedType dataFeedType)
        {
            var item = this.vkGroupRepository.GetProcessingState(vkGroupId, dataFeedType);
            return item != null;
        }

        public int GetMonthLimit()
        {
            return this.configurationProvider.GetConfigurationSection<FeedProcessingConfig>().MonthToFetch;
        }
    }
}