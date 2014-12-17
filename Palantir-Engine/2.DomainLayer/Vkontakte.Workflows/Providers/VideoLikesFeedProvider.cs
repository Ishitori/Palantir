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

    public class VideoLikesFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IVideoRepository videoRepository;
        private readonly IVkResponseMapper responseMapper;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public VideoLikesFeedProvider(ILog log, IVideoRepository videoRepository, IVkResponseMapper responseMapper, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.videoRepository = videoRepository;
            this.responseMapper = responseMapper;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.VideoLikes;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.VideoLikes;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);
            IList<Video> videos = this.videoRepository.GetVideosByVkGroupId(vkGroup.Id).Where(v => !dateLimit.HasValue || v.PostedDate > dateLimit).ToList();

            foreach (var video in videos)
            {
                var videoLikes = dataProvider.GetLikes(vkGroup.Id.ToString(), video.VkId, LikeShareType.Video, 0);

                if (videoLikes == null)
                {
                    continue;
                }

                this.log.DebugFormat("Video comments feed is received: {0}", videoLikes.Feed);
                videoLikes.ParentObjectId = video.VkId;
                string newFeed = this.responseMapper.MapResponseObject(videoLikes);

                DataFeed dataFeed = new DataFeed
                {
                    ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                    Feed = newFeed,
                    VkGroupId = vkGroup.Id,
                    RelatedObjectId = video.VkId,
                    Type = DataFeedType.VideoLikes
                };

                yield return dataFeed;
            }
        }
    }
}