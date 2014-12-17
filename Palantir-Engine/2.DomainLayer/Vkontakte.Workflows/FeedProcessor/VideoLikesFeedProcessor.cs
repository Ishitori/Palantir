namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed;

    public class VideoLikesFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IVideoRepository videoRepository;

        public VideoLikesFeedProcessor(IVkResponseMapper responseMapper, IVideoRepository videoRepository, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.videoRepository = videoRepository;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);
            this.ProcessVideo(feed, group);
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessVideo(response feed, VkGroup group)
        {
            var savedVideo = this.videoRepository.GetVideo(group.Id, feed.ParentObjectId);

            if (savedVideo != null)
            {
                this.UpdateExistingVideo(feed, savedVideo);
            }
        }

        private void UpdateExistingVideo(response feed, Video savedVideo)
        {
            this.log.DebugFormat("Post with VkId={0} is already in database", feed.ParentObjectId);
            int newLikesCount = int.Parse(feed.count);

            if (savedVideo.LikesCount != newLikesCount)
            {
                savedVideo.LikesCount = newLikesCount;
                this.videoRepository.Update(savedVideo);
                this.log.DebugFormat("Video with VkId={0} comments or likes changed. Updating the post", feed.ParentObjectId);
            }
        }    
    }
}