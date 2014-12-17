namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;

    using Ix.Palantir.Configuration;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;

    public class ProcessingStrategy : IProcessingStrategy
    {
        private readonly IConfigurationProvider configurationProvider;
        private readonly IVkGroupRepository vkGroupRepository;
        private readonly IDateTimeHelper dateTimeHelper;

        public ProcessingStrategy(IConfigurationProvider configurationProvider, IVkGroupRepository vkGroupRepository, IDateTimeHelper dateTimeHelper)
        {
            this.configurationProvider = configurationProvider;
            this.vkGroupRepository = vkGroupRepository;
            this.dateTimeHelper = dateTimeHelper;
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

        public DateTime? GetDateLimit(int vkGroupId, DataFeedType feedType)
        {
            DateTime? dateLimit = this.IsLimitedProcessingEnabled(vkGroupId, feedType)
                ? this.dateTimeHelper.GetDateTimeNow().Date.AddMonths(-this.GetMonthLimit())
                : (DateTime?)null;

            return dateLimit;
        }
    }
}