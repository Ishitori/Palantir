namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;
    using Utilities;

    internal class WallPostCommentsFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IPostRepository postRepository;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public WallPostCommentsFeedProvider(ILog log, IPostRepository postRepository, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.postRepository = postRepository;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.WallPostComments;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.WallPostComments;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);
            IList<Post> posts = this.postRepository.GetPostsByVkGroupId(vkGroup.Id).Where(p => !dateLimit.HasValue || p.PostedDate > dateLimit).ToList();

            foreach (var post in posts)
            {
                int offsetCounter = 0;

                while (true)
                {
                    var wallPostComments = dataProvider.GetWallPostComments(post.VkId, vkGroup.Id.ToString(), offsetCounter);
                    this.log.DebugFormat("Post comments feed is received: {0}", wallPostComments.Feed);

                    if (wallPostComments.comment == null || wallPostComments.comment.Length == 0)
                    {
                        break;
                    }

                    // if last comment in this pack is was created before dateLimit, we ignore the whole pack, but still trying to find the latest one
                    if (dateLimit.HasValue && wallPostComments.comment[wallPostComments.comment.Length - 1].date.FromUnixTimestamp() < dateLimit.Value)
                    {
                        continue;
                    }
                    
                    DataFeed dataFeed = new DataFeed
                    {
                        ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                        Feed = wallPostComments.Feed,
                        VkGroupId = vkGroup.Id,
                        RelatedObjectId = post.VkId,
                        Type = DataFeedType.WallPostComments
                    };

                    yield return dataFeed;

                    offsetCounter += wallPostComments.comment.Length;
                }
            }
        }
    }
}