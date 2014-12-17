namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using API;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Ix.Palantir.Vkontakte.API.Responses.VideoComments;
    using Logging;
    using Utilities;
    using response = API.Responses.VideoComments.response;

    public class VideoCommentFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IVideoRepository videoRepository;
        private readonly IProcessingStrategy processingStrategy;

        public VideoCommentFeedProcessor(IVkResponseMapper responseMapper, IVideoRepository videoRepository, IProcessingStrategy processingStrategy, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.videoRepository = videoRepository;
            this.processingStrategy = processingStrategy;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (responseComment item in feed.comment)
            {
                this.ProcessPost(dataFeed.RelatedObjectId, item, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessPost(string vkVideoId, responseComment comment, VkGroup group)
        {
            var savedComment = this.videoRepository.GetVideoCommentByVkGroupId(group.Id, comment.id);

            if (savedComment != null)
            {
                this.log.DebugFormat("Video comment with VkId={0} is already in database", comment.id);
                return;
            }

            if (this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.VideoComments) &&
                comment.date.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched video comment with VkId={0} is created more than {1} months ago. Skipping.", comment.id, this.processingStrategy.GetMonthLimit());
                return;
            }

            savedComment = new VideoComment
            {
                VkId = comment.id,
                VkVideoId = vkVideoId,
                VkGroupId = group.Id,
                PostedDate = comment.date.FromUnixTimestamp(),
                CreatorId = long.Parse(comment.from_id)
            };

            this.videoRepository.SaveComment(savedComment);
            this.log.DebugFormat("Topic comment with VkId={0} is not found in database. Saved with Id={1}", savedComment.VkId, savedComment.Id);
        }
    }
}