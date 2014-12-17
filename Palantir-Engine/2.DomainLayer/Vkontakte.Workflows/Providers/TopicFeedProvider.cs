namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;
    using Logging;
    using Utilities;

    public class TopicFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public TopicFeedProvider(ILog log, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType 
        { 
            get
            {
                return QueueItemType.Topic;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.Topic;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            int offsetCounter = 0;
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);

            while (true)
            {
                var topicFeed = dataProvider.GetTopics(vkGroup.Id.ToString(), offsetCounter);
                this.log.DebugFormat("Topics feed is received: {0}", topicFeed.Feed);

                if (topicFeed.topics == null || topicFeed.topics.Length == 0 || topicFeed.topics[0].topic == null || topicFeed.topics[0].topic.Length == 0)
                {
                    break;
                }

                DataFeed dataFeed = new DataFeed
                {
                    ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                    Feed = topicFeed.Feed,
                    VkGroupId = vkGroup.Id,
                    Type = DataFeedType.Topic
                };

                yield return dataFeed;

                offsetCounter += topicFeed.topics[0].topic.Length;

                if (dateLimit.HasValue && topicFeed.topics[topicFeed.topics.Length - 1].topic[0].created.FromUnixTimestamp() < dateLimit)
                {
                    break;
                }
            }
        }
    }
}