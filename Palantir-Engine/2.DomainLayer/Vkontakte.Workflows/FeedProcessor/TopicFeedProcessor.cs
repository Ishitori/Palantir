namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;
    using API;
    using API.Responses.GroupTopics;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Logging;
    using Utilities;

    public class TopicFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IProcessingStrategy processingStrategy;
        private readonly ITopicRepository topicRepository;

        public TopicFeedProcessor(ILog log, IVkResponseMapper responseMapper, IProcessingStrategy processingStrategy, ITopicRepository topicRepository)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.processingStrategy = processingStrategy;
            this.topicRepository = topicRepository;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var topic in feed.topics)
            {
                foreach (var topicsTopic in topic.topic)
                {
                    this.ProcessTopic(topicsTopic, group);
                }
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessTopic(responseTopicsTopic topic, VkGroup group)
        {
            var savedTopic = this.topicRepository.GetTopic(group.Id, topic.tid);

            if (savedTopic == null && 
                this.processingStrategy.IsLimitedProcessingEnabled(group.Id, DataFeedType.Topic) &&
                topic.created.FromUnixTimestamp().AddMonths(this.processingStrategy.GetMonthLimit()) < DateTime.UtcNow)
            {
                this.log.DebugFormat("Fetched topic with VkId={0} is created more than {1} months ago. Skipping.", topic.tid, this.processingStrategy.GetMonthLimit());
                return;
            }

            if (savedTopic != null)
            {
                this.UpdateExistingTopic(savedTopic, topic);
            }
            else
            {
                this.SaveNewTopic(topic, group);
            }
        }

        private void SaveNewTopic(responseTopicsTopic responseTopic, VkGroup group)
        {
            Topic topic = new Topic
                {
                    VkId = responseTopic.tid,
                    VkGroupId = group.Id,
                    PostedDate = responseTopic.created.FromUnixTimestamp(),
                    LastCommentDate = responseTopic.updated.FromUnixTimestamp(),
                    Title = responseTopic.title,
                    CommentsCount = int.Parse(responseTopic.comments),
                    CreatedByVkId = long.Parse(responseTopic.created_by),
                };

            this.topicRepository.Save(topic);
            this.log.DebugFormat("Topic with VkId={0} is not found in database. Saved with Id={1}", topic.VkId, topic.Id);
        }

        private void UpdateExistingTopic(Topic savedTopic, responseTopicsTopic topicData)
        {
            this.log.DebugFormat("Topic with VkId={0} is already in database", topicData.tid);
            int newCommentsCount = int.Parse(topicData.comments);
            DateTime lastUpdatedDate = topicData.updated.FromUnixTimestamp();

            if (savedTopic.CommentsCount != newCommentsCount || savedTopic.LastCommentDate != lastUpdatedDate)
            {
                savedTopic.CommentsCount = newCommentsCount;
                savedTopic.LastCommentDate = lastUpdatedDate;
                this.topicRepository.UpdateTopic(savedTopic);
                this.log.DebugFormat("Topic with VkId={0} comments changed. Updating the topic", savedTopic.VkId);
            }
        }
    }
}