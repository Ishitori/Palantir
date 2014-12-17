namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.WallPostComments;

    public class WallPostCommentFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IPostRepository postRepository;
        private readonly IProcessingStrategy processingStrategy;

        public WallPostCommentFeedProcessor(IVkResponseMapper responseMapper, IPostRepository postRepository, IProcessingStrategy processingStrategy, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.postRepository = postRepository;
            this.processingStrategy = processingStrategy;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var comment in feed.comment)
            {
                this.ProcessPost(dataFeed.RelatedObjectId, comment, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessPost(string vkPostId, responseComment comment, VkGroup group)
        {
            var savedComment = this.postRepository.GetPostComment(group.Id, comment.cid);

            if (savedComment != null)
            {
                this.log.DebugFormat("Post comment with VkId={0} is already in database", comment.cid);
                return;
            }

            if (this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.WallPostComments) &&
                comment.date.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched post comment with VkId={0} is created more than {1} months ago. Skipping.", comment.cid, this.processingStrategy.GetMonthLimit());
                return;
            }

            savedComment = new PostComment
            {
                VkId = comment.cid,
                VkPostId = vkPostId,
                VkGroupId = group.Id,
                PostedDate = comment.date.FromUnixTimestamp(),
                CreatorId = long.Parse(comment.uid),
                ReplyToUserId = string.IsNullOrWhiteSpace(comment.reply_to_uid) ? null : (long?)long.Parse(comment.reply_to_uid),
                ReplyToVkId = string.IsNullOrWhiteSpace(comment.reply_to_cid) ? null : (int?)int.Parse(comment.reply_to_cid),
            };

            this.postRepository.SaveComment(savedComment);
            this.log.DebugFormat("Post comment with VkId={0} is not found in database. Saved with Id={1}", savedComment.VkId, savedComment.Id);
        }
    }
}