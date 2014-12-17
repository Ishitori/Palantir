namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API.Access;

    public class MembersCountFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;

        public MembersCountFeedProvider(ILog log, IDateTimeHelper dateTimeHelper)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.MembersCount;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.MembersCount;
            }
        }

        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            var memberFeed = dataProvider.GetGroupMemberIds(vkGroup.Id.ToString(), 0);
            this.log.DebugFormat("Users feed is received: {0}", memberFeed.Feed);

            DataFeed dataFeed = new DataFeed
            {
                ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                Feed = memberFeed.Feed,
                VkGroupId = vkGroup.Id,
                Type = DataFeedType.MembersCount
            };

            yield return dataFeed;
        }
    }
}