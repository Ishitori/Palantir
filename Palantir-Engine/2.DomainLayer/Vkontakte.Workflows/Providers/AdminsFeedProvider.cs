namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Logging;

    public class AdminsFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;

        public AdminsFeedProvider(ILog log, IDateTimeHelper dateTimeHelper)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.Administrators;
            }
        }

        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.Administrators;
            }
        }

        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            var admins = dataProvider.GetGroupAministrators((-vkGroup.Id).ToString(), (Ix.Palantir.Vkontakte.API.VkGroupType)(int)vkGroup.Type);
            this.log.DebugFormat("Administrators feed received");

            if (string.IsNullOrWhiteSpace(admins.Page))
            {
                return new List<DataFeed>();
            }

            DataFeed dataFeed = new DataFeed
            {
                ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                Feed = admins.Page,
                VkGroupId = vkGroup.Id,
                Type = DataFeedType.Administrators
            };

            return new List<DataFeed> { dataFeed };
        }
    }
}