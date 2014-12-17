namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API.Responses.GroupTopicComments;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;
    using Logging;
    using Utilities;

    public class TopicCommentFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly ITopicRepository topicRepository;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public TopicCommentFeedProvider(ILog log, ITopicRepository topicRepository, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.topicRepository = topicRepository;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.TopicComments;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.TopicComment;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);
            IList<Topic> topics = this.topicRepository.GetTopicsByVkGroupId(vkGroup.Id).Where(p => !dateLimit.HasValue || p.PostedDate > dateLimit).ToList();

            foreach (var topic in topics)
            {
                int offsetCounter = 0;

                while (true)
                {
                    var topicComments = dataProvider.GetTopicComments(vkGroup.Id.ToString(), topic.VkId, offsetCounter);
                    this.log.DebugFormat("Topic comments feed is received: {0}", topicComments.Feed);

                    if (topicComments.Items == null || topicComments.Items.Length == 0 || topicComments.Items[0].comment == null || topicComments.Items[0].comment.Length == 0)
                    {
                        break;
                    }

                    // if last comment in this pack is was created before dateLimit, we ignore the whole pack, but still trying to find the latest one
                    responseComments lastCommentGroup = topicComments.Items[topicComments.Items.Length - 1];

                    if (dateLimit.HasValue && lastCommentGroup.comment[lastCommentGroup.comment.Length - 1].date.FromUnixTimestamp() < dateLimit.Value)
                    {
                        continue;
                    }

                    DataFeed dataFeed = new DataFeed
                    {
                        ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                        Feed = topicComments.Feed,
                        VkGroupId = vkGroup.Id,
                        RelatedObjectId = topic.VkId,
                        Type = DataFeedType.TopicComment
                    };

                    yield return dataFeed;

                    offsetCounter += topicComments.Items[0].comment.Length;
                }
            }
        }
    }
}