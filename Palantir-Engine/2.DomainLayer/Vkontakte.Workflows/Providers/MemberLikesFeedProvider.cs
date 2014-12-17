namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;

    public class MemberLikesFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IListRepository listRepository;
        private readonly IVkResponseMapper responseMapper;
        private readonly IProcessingStrategy strategy;

        public MemberLikesFeedProvider(ILog log, IDateTimeHelper dateTimeHelper, IListRepository listRepository, IVkResponseMapper responseMapper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
            this.listRepository = listRepository;
            this.responseMapper = responseMapper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get { return QueueItemType.MemberLikes; }
        }
        public DataFeedType ProvidedDataType
        {
            get { return DataFeedType.MemberLikes; }
        }

        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);
            IList<LikeShareType> types = Enum.GetValues(typeof(LikeShareType)).Cast<LikeShareType>().Where(t => t != LikeShareType.Undefined).ToList();

            foreach (var type in types)
            {
                int offsetCounter = 0;
                IEnumerable<string> entities = this.GetEntityVkIds(type, vkGroup, dateLimit);

                foreach (var entity in entities)
                {
                    var dataFeed = this.GetFeeds(dataProvider, type, entity, vkGroup, ref offsetCounter);

                    if (dataFeed == null)
                    {
                        continue;
                    }

                    yield return dataFeed;
                }
            }
        }

        private DataFeed GetFeeds(IVkDataProvider dataProvider, LikeShareType type, string entityVkId, VkGroup vkGroup, ref int offsetCounter)
        {
            var likesFeed = dataProvider.GetLikes(vkGroup.Id.ToString(), entityVkId, type, offsetCounter);

            if (likesFeed == null)
            {
                return null;
            }

            this.log.DebugFormat("Like feed is received: {0}", likesFeed.Feed);
            likesFeed.ParentObjectId = entityVkId;
            likesFeed.LikeShareType = (int)type;
            string newFeed = this.responseMapper.MapResponseObject(likesFeed);

            var dataFeed = new DataFeed
            {
                ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                Feed = newFeed,
                VkGroupId = vkGroup.Id,
                RelatedObjectId = entityVkId,
                Type = DataFeedType.MemberLikes
            };

            if (likesFeed.users[0].uid == null || likesFeed.users[0].uid.Length == 0)
            {
                return null;
            }

            offsetCounter += likesFeed.users.Length;
            return dataFeed;
        }

        private IEnumerable<string> GetEntityVkIds(LikeShareType type, VkGroup vkGroup, DateTime? dateLimit)
        {
            switch (type)
            {
                case LikeShareType.Post:
                    return this.listRepository.GetPostVkIds(vkGroup.Id, dateLimit);
                
                case LikeShareType.Comment:
                    return this.listRepository.GetPostCommentVkIds(vkGroup.Id, dateLimit);
                
                case LikeShareType.Photo:
                    return this.listRepository.GetPhotoVkIds(vkGroup.Id, dateLimit);
                
                case LikeShareType.Video:
                    return this.listRepository.GetVideoVkIds(vkGroup.Id, dateLimit);

                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}