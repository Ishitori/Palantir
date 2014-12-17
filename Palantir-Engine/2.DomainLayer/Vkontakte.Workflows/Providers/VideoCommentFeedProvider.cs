namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.API.Responses.VideoComments;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;

    using Logging;

    public class VideoCommentFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IVideoRepository videoRepository;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public VideoCommentFeedProvider(ILog log, IVideoRepository videoRepository, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.videoRepository = videoRepository;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.VideoComments;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.VideoComments;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);
            IList<Video> videos = this.videoRepository.GetVideosByVkGroupId(vkGroup.Id).Where(v => !dateLimit.HasValue || v.PostedDate > dateLimit).ToList();

            foreach (var video in videos)
            {
                int offsetCounter = 0;

                while (true)
                {
                    var videoComments = dataProvider.GetVideoComments(vkGroup.Id.ToString(), video.VkId, offsetCounter);
                    this.log.DebugFormat("Video comments feed is received: {0}", videoComments.Feed);

                    if (videoComments.comment == null || videoComments.comment.Length == 0)
                    {
                        break;
                    }

                    // if last comment in this pack is was created before dateLimit, we ignore the whole pack, but still trying to find the latest one
                    responseComment lastCommentGroup = videoComments.comment[videoComments.comment.Length - 1];

                    if (dateLimit.HasValue && lastCommentGroup.date.FromUnixTimestamp() < dateLimit.Value)
                    {
                        continue;
                    }

                    DataFeed dataFeed = new DataFeed
                    {
                        ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                        Feed = videoComments.Feed,
                        VkGroupId = vkGroup.Id,
                        RelatedObjectId = video.VkId,
                        Type = DataFeedType.VideoComments
                    };

                    yield return dataFeed;

                    offsetCounter += videoComments.comment.Length;
                }
            }
        }
    }
}