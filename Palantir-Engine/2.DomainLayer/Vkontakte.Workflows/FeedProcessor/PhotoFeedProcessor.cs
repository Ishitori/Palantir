namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.Photos;

    public class PhotoFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IPhotoRepository photoRepository;
        private readonly IProcessingStrategy processingStrategy;

        public PhotoFeedProcessor(IVkResponseMapper responseMapper, IPhotoRepository photoRepository, IProcessingStrategy processingStrategy, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.photoRepository = photoRepository;
            this.processingStrategy = processingStrategy;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var photo in feed.photo)
            {
                this.ProcessPhoto(photo, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessPhoto(responsePhoto photo, VkGroup group)
        {
            var savedPhoto = this.photoRepository.GetPhotoByIdInAlbum(group.Id, photo.aid, photo.pid);

            if (savedPhoto != null)
            {
                this.log.DebugFormat("Photo with VkId={0} is already in database", photo.pid);
                return;
            }

            if (this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.Photo) &&
                photo.created.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched photo with VkId={0} is created more than {1} months ago. Skipping.", photo.pid, this.processingStrategy.GetMonthLimit());
                return;
            }

            savedPhoto = new Photo
            {
                VkGroupId = group.Id,
                PostedDate = photo.created.FromUnixTimestamp(),
                VkId = photo.pid,
                AlbumId = photo.aid,
                Text = photo.text
            };

            this.photoRepository.Save(savedPhoto);
            this.log.DebugFormat("Photo with VkId={0} is not found in database. Saved with Id={1}", savedPhoto.VkId, savedPhoto.Id);
        }
    }
}