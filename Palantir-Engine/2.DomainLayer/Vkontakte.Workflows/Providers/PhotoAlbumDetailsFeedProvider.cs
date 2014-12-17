namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API.Access;

    public class PhotoAlbumDetailsFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IPhotoRepository photoRepository;
        private readonly IDateTimeHelper dateTimeHelper;

        public PhotoAlbumDetailsFeedProvider(IPhotoRepository photoRepository, IDateTimeHelper dateTimeHelper, ILog log)
        {
            this.photoRepository = photoRepository;
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
        }

        public QueueItemType SupportedFeedType 
        {
            get
            {
                return QueueItemType.PhotoAlbumDetails;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.PhotoAlbumDetails;
            }
        }

        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            IList<string> albumIds = this.photoRepository.GetGroupAlbumIds(vkGroup.Id);

            foreach (var albumId in albumIds)
            {
                var photos = dataProvider.GetPhotoAlbumDetails(vkGroup.Id.ToString(), albumId, 0);
                this.log.DebugFormat("Photos feed is received: {0}", photos.Feed);

                if (photos.photo == null || photos.photo.Length == 0)
                {
                    continue;
                }

                DataFeed dataFeed = new DataFeed
                {
                    ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                    Feed = photos.Feed,
                    VkGroupId = vkGroup.Id,
                    Type = DataFeedType.PhotoAlbumDetails
                };

                yield return dataFeed;
            }
        }
    }
}