namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;
    using Utilities;

    internal class WallPostFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public WallPostFeedProvider(ILog log, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.WallPosts;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.WallPosts;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            int offsetCounter = 0;
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);

            while (true)
            {
                var posts = dataProvider.GetWallPosts(vkGroup.Id.ToString(), offsetCounter);
                this.log.DebugFormat("Posts feed is received: {0}", posts.Feed);

                if (posts.post == null || posts.post.Length == 0)
                {
                    break;
                }

                DataFeed dataFeed = new DataFeed
                {
                    ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                    Feed = posts.Feed,
                    VkGroupId = vkGroup.Id,
                    Type = DataFeedType.WallPosts
                };

                yield return dataFeed;

                offsetCounter += posts.post.Length;

                if (dateLimit.HasValue && posts.post[posts.post.Length - 1].date.FromUnixTimestamp() < dateLimit)
                {
                    break;
                }
            }
        }
    }
}