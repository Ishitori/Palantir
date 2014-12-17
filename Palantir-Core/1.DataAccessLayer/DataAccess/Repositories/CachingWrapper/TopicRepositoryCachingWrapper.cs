namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class TopicRepositoryCachingWrapper : ITopicRepository
    {
        private readonly ITopicRepository topicRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;

        public TopicRepositoryCachingWrapper(ITopicRepository topicRepository, IDataGatewayProvider dataGatewayProvider, IFeedProcessingCachingStrategy cachingStrategy)
        {
            this.topicRepository = topicRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.cachingStrategy = cachingStrategy;
        }

        public void Save(Topic topic)
        {
            try
            {
                this.topicRepository.Save(topic);
                this.cachingStrategy.StoreItem(topic);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, topic, "topic");
            }
        }
        public void UpdateTopic(Topic topic)
        {
            this.topicRepository.UpdateTopic(topic);
            this.cachingStrategy.StoreItem(topic);
        }

        public Topic GetTopic(int vkGroupId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<Topic>(vkGroupId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetCacheItems(vkGroupId));
            return this.cachingStrategy.GetItem<Topic>(vkGroupId, vkId);
        }
        public IList<Topic> GetTopicsByVkGroupId(int vkGroupId)
        {
            return this.GetTopics(vkGroupId).Cast<Topic>().ToList();
        }

        public void SaveComment(TopicComment comment)
        {
            try
            {
                this.topicRepository.SaveComment(comment);
                this.cachingStrategy.StoreItem(comment);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, comment, "topiccomment");
            }
        }
        public void UpdateComment(TopicComment comment)
        {
            this.topicRepository.UpdateComment(comment);
            this.cachingStrategy.StoreItem(comment);
        }

        public TopicComment GetTopicComment(int vkGroupId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<TopicComment>(vkGroupId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetCacheItems(vkGroupId));
            return this.cachingStrategy.GetItem<TopicComment>(vkGroupId, vkId);
        }
        public IList<TopicComment> GetTopicCommentsByVkGroupId(int vkGroupId)
        {
            return this.GetTopicsComment(vkGroupId).Cast<TopicComment>().ToList();
        }

        private IEnumerable<IVkEntity> GetTopics(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<Topic> topics = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.Topic)
                                         ? dataGateway.Connection.Query<Topic>("select * from topic where vkgroupid = @vkgroupid and posteddate > @postedDate", new { vkgroupid = vkGroupId, postedDate = this.cachingStrategy.GetDateLimit() })
                                         : dataGateway.Connection.Query<Topic>("select * from topic where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId });

                return topics;
            }
        }
        private IEnumerable<IVkEntity> GetTopicsComment(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<TopicComment> topicComments = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.TopicComment)
                                         ? dataGateway.Connection.Query<TopicComment>("select tc.* from topiccomment tc where tc.vkgroupid = @vkgroupid and tc.posteddate > @postedDate", new { vkgroupid = vkGroupId, postedDate = this.cachingStrategy.GetDateLimit() })
                                         : dataGateway.Connection.Query<TopicComment>("select * from topiccomment where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId });

                return topicComments;
            }
        }
        private IEnumerable<IVkEntity> GetCacheItems(int vkGroupId)
        {
            var items = new List<IVkEntity>();

            items.AddRange(this.GetTopics(vkGroupId));
            items.AddRange(this.GetTopicsComment(vkGroupId));

            return items;
        }

        private string GetInitCacheKey(int vkGroupId)
        {
            return string.Format("VK_Topic_Processing_VkGroup_{0}", vkGroupId);
        }
    }
}