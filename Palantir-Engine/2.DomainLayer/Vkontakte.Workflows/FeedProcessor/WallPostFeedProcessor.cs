namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.WallPosts;

    public class WallPostFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IPostRepository postRepository;
        private readonly IProcessingStrategy processingStrategy;

        public WallPostFeedProcessor(IVkResponseMapper responseMapper, IPostRepository postRepository, IProcessingStrategy processingStrategy, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.postRepository = postRepository;
            this.processingStrategy = processingStrategy;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var post in feed.post)
            {
                this.ProcessPost(post, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessPost(responsePost post, VkGroup group)
        {
            var savedPost = this.postRepository.GetPost(group.Id, post.id);

            if (savedPost == null && 
                this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.WallPosts) &&
                post.date.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched post with VkId={0} is created more than {1} months ago. Skipping.", post.id, this.processingStrategy.GetMonthLimit());
                return;
            }

            if (savedPost != null)
            {
                this.UpdateExistingPost(post, savedPost);
            }
            else
            {
                this.SaveNewPost(post, group);
            }
        }

        private void SaveNewPost(responsePost post, VkGroup group)
        {
            Post savedPost = new Post
            {
                VkGroupId = group.Id,
                PostedDate = post.date.FromUnixTimestamp(),
                CreatorId = long.Parse(post.from_id),
                LikesCount = int.Parse(post.likes[0].count),
                CommentsCount = int.Parse(post.comments[0].count),
                Text = post.text,
                VkId = post.id
            };

            this.postRepository.Save(savedPost);
            this.log.DebugFormat("Post with VkId={0} is not found in database. Saved with Id={1}", savedPost.VkId, savedPost.Id);
        }
        private void UpdateExistingPost(responsePost post, Post savedPost)
        {
            this.log.DebugFormat("Post with VkId={0} is already in database", post.id);
            int newLikesCount = int.Parse(post.likes[0].count);
            int newCommentsCount = int.Parse(post.comments[0].count);

            if (savedPost.LikesCount != newLikesCount || savedPost.CommentsCount != newCommentsCount)
            {
                savedPost.LikesCount = newLikesCount;
                savedPost.CommentsCount = newCommentsCount;
                this.postRepository.UpdatePost(savedPost);
                this.log.DebugFormat("Post with VkId={0} comments or likes changed. Updating the post", post.id);
            }
        }
    }
}