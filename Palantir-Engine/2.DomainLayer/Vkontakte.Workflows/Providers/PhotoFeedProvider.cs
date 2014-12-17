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

    internal class PhotoFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProcessingStrategy strategy;

        public PhotoFeedProvider(ILog log, IDateTimeHelper dateTimeHelper, IProcessingStrategy strategy)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
            this.strategy = strategy;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.Photo;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.Photo;
            }
        }

        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            int offsetCounter = 0;
            DateTime? dateLimit = this.strategy.GetDateLimit(vkGroup.Id, this.ProvidedDataType);

            while (true)
            {
                var photos = dataProvider.GetPhotos(vkGroup.Id.ToString(), offsetCounter);
                this.log.DebugFormat("Photo feed is received: {0}", photos.Feed);

                if (photos.photo == null || photos.photo.Length == 0)
                {
                    break;
                }

                DataFeed dataFeed = new DataFeed
                {
                    ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                    Feed = photos.Feed,
                    VkGroupId = vkGroup.Id,
                    Type = DataFeedType.Photo
                };

                yield return dataFeed;

                offsetCounter += photos.photo.Length;

                if (dateLimit.HasValue && photos.photo[photos.photo.Length - 1].created.FromUnixTimestamp() < dateLimit.Value)
                {
                    break;
                }
            }
        }
    }
}