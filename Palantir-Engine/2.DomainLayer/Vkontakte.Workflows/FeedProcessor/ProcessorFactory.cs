namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using System.Collections.Generic;
    using DomainModel;
    using Framework.ObjectFactory;

    public class ProcessorFactory : IProcessorFactory
    {
        private readonly IDictionary<DataFeedType, IFeedProcessor> processorsCache;

        public ProcessorFactory()
        {
            this.processorsCache = new Dictionary<DataFeedType, IFeedProcessor>();
        }

        public IFeedProcessor Create(DataFeedType feedType)
        {
            if (this.processorsCache.ContainsKey(feedType))
            {
                return this.processorsCache[feedType];
            }

            switch (feedType)
            {
                case DataFeedType.WallPosts:
                    this.processorsCache.Add(feedType, Factory.GetInstance<WallPostFeedProcessor>());
                    break;

                case DataFeedType.WallPostComments:
                    this.processorsCache.Add(feedType, Factory.GetInstance<WallPostCommentFeedProcessor>());
                    break;

                case DataFeedType.Photo:
                    this.processorsCache.Add(feedType, Factory.GetInstance<PhotoFeedProcessor>());
                    break;

                case DataFeedType.PhotoAlbumDetails:
                    this.processorsCache.Add(feedType, Factory.GetInstance<PhotoAlbumDetailsFeedProcessor>());
                    break;

                case DataFeedType.MembersCount:
                    this.processorsCache.Add(feedType, Factory.GetInstance<MembersCountFeedProcessor>());
                    break;

                case DataFeedType.MemberLikes:
                    this.processorsCache.Add(feedType, Factory.GetInstance<MemberLikesFeedProcessor>());
                    break;

                case DataFeedType.MemberShares:
                    this.processorsCache.Add(feedType, Factory.GetInstance<MemberSharesFeedProcessor>());
                    break;

                case DataFeedType.MemberSubscription:
                    this.processorsCache.Add(feedType, Factory.GetInstance<MemberSubscriptionFeedProcessor>());
                    break;

                case DataFeedType.Video:
                    this.processorsCache.Add(feedType, Factory.GetInstance<VideoFeedProcessor>());
                    break;

                case DataFeedType.VideoComments:
                    this.processorsCache.Add(feedType, Factory.GetInstance<VideoCommentFeedProcessor>());
                    break;

                case DataFeedType.VideoLikes:
                    this.processorsCache.Add(feedType, Factory.GetInstance<VideoLikesFeedProcessor>());
                    break;

                case DataFeedType.Administrators:
                    this.processorsCache.Add(feedType, Factory.GetInstance<AdministratorsFeedProcessor>());
                    break;

                case DataFeedType.MembersInfo:
                    this.processorsCache.Add(feedType, Factory.GetInstance<MembersFeedProcessor>());
                    break;

                case DataFeedType.Topic:
                    this.processorsCache.Add(feedType, Factory.GetInstance<TopicFeedProcessor>());
                    break;

                case DataFeedType.TopicComment:
                    this.processorsCache.Add(feedType, Factory.GetInstance<TopicCommentFeedProcessor>());
                    break;

                default:
                    throw new ArgumentException(string.Format("Feed type \"{0}\" is not supported", feedType), "feedType");
            }

            return this.processorsCache[feedType];
        }
    }
}