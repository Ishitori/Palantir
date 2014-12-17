namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using API;
    using API.Responses.GroupTopicComments;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Logging;
    using Utilities;
    using response = API.Responses.GroupTopicComments.response;

    public class TopicCommentFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly ITopicRepository topicRepository;
        private readonly IProcessingStrategy processingStrategy;

        public TopicCommentFeedProcessor(IVkResponseMapper responseMapper, ITopicRepository topicRepository, IProcessingStrategy processingStrategy, ILog log)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.topicRepository = topicRepository;
            this.processingStrategy = processingStrategy;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var item in feed.Items)
            {
                if (item != null)
                {
                    foreach (var comment in item.comment)
                    {
                        this.ProcessPost(dataFeed.RelatedObjectId, comment, group);
                    }
                }
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessPost(string vkTopicId, responseCommentsComment comment, VkGroup group)
        {
            var savedComment = this.topicRepository.GetTopicComment(group.Id, comment.id);

            if (savedComment != null)
            {
                this.log.DebugFormat("Topic comment with VkId={0} is already in database", comment.id);
                return;
            }

            if (this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.TopicComment) &&
                comment.date.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched topic comment with VkId={0} is created more than {1} months ago. Skipping.", comment.id, this.processingStrategy.GetMonthLimit());
                return;
            }

            savedComment = new TopicComment
            {
                VkId = comment.id,
                VkTopicId = vkTopicId,
                VkGroupId = group.Id,
                PostedDate = comment.date.FromUnixTimestamp(),
                CreatorId = long.Parse(comment.from_id)
            };

            this.topicRepository.SaveComment(savedComment);
            this.log.DebugFormat("Topic comment with VkId={0} is not found in database. Saved with Id={1}", savedComment.VkId, savedComment.Id);
        }
    }
}