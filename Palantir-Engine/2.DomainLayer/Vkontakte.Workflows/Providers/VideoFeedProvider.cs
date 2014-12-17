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

    internal class VideoFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public VideoFeedProvider(ILog log, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.Video;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.Video;
            }
        }
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            int offsetCounter = 0;
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);

            while (true)
            {
                var videos = dataProvider.GetVideos(vkGroup.Id.ToString(), offsetCounter);
                this.log.DebugFormat("Video feed is received: {0}", videos.Feed);

                if (videos.video == null || videos.video.Length == 0)
                {
                    break;
                }

                DataFeed dataFeed = new DataFeed()
                {
                    ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                    Feed = videos.Feed,
                    VkGroupId = vkGroup.Id,
                    Type = DataFeedType.Video
                };

                yield return dataFeed;

                offsetCounter += videos.video.Length;

                if (dateLimit.HasValue && videos.video[videos.video.Length - 1].date.FromUnixTimestamp() < dateLimit.Value)
                {
                    break;
                }
            }
        }
    }
}