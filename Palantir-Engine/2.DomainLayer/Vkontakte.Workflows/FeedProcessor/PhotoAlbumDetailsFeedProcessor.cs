namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.PhotoAlbumDetailsResponse;

    public class PhotoAlbumDetailsFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IPhotoRepository photoRepository;

        public PhotoAlbumDetailsFeedProcessor(IVkResponseMapper responseMapper, IPhotoRepository photoRepository, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.photoRepository = photoRepository;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var photo in feed.photo)
            {
                this.ProcessPhotoDetails(photo, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessPhotoDetails(responsePhoto photo, VkGroup group)
        {
            var savedPhoto = this.photoRepository.GetPhotoByIdInAlbum(group.Id, photo.aid, photo.pid);

            if (savedPhoto == null)
            {
                this.log.DebugFormat("Photo with VkId={0} is not in database", photo.pid);
                return;
            }

            int commentsCount = int.Parse(photo.comments[0].count);
            int likesCount = int.Parse(photo.likes[0].count);

            if (savedPhoto.CommentsCount != commentsCount || savedPhoto.LikesCount != likesCount)
            {
                savedPhoto.CommentsCount = commentsCount;
                savedPhoto.LikesCount = likesCount;
                this.photoRepository.UpdatePhoto(savedPhoto);
                this.log.DebugFormat("Photo with VkId={0} comments and likes are updated.", savedPhoto.VkId);
            }
        }
    }
}