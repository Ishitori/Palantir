namespace Ix.Palantir.DataAccess.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class TopicRepository : ITopicRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public TopicRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void Save(Topic topic)
        {
            if (!topic.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                topic.Id = dataGateway.Connection.Query<int>(@"insert into topic(vkgroupid, posteddate, vkid, year, month, week, day, hour, minute, second, createdbyvkid, title, commentscount, lastcommentdate) values (@VkGroupId, @PostedDate, @VkId, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @CreatedByVkId, @Title, @CommentsCount, @LastCommentDate) RETURNING id", topic).First();
            }
        }
        public void UpdateTopic(Topic topic)
        {
            if (topic == null)
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"update topic set vkgroupid = @VkGroupId, posteddate = @PostedDate, vkid = @VkId, year = @Year, month = @Month, week = @Week, day = @Day, hour = @Hour, minute = @Minute, second = @Second, createdbyvkid = @CreatedByVkId, title = @Title, commentscount = @CommentsCount, lastcommentdate = @LastCommentDate where id = @Id", topic);
            }
        }

        public Topic GetTopic(int vkGroupId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Topic>().SingleOrDefault(p => p.VkGroupId == vkGroupId && p.VkId == vkId);
            }
        }
        public IList<Topic> GetTopicsByVkGroupId(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Topic>().Where(p => p.VkGroupId == vkGroupId).ToList();
            }
        }

        public TopicComment GetTopicComment(int vkGroupId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<TopicComment>().SingleOrDefault(p => p.VkGroupId == vkGroupId && p.VkId == vkId);
            }
        }
        public IList<TopicComment> GetTopicCommentsByVkGroupId(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<TopicComment>().Where(p => p.VkGroupId == vkGroupId).ToList();
            }
        }

        public void SaveComment(TopicComment comment)
        {
            if (!comment.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                comment.Id = dataGateway.Connection.Query<int>(@"insert into topiccomment(vkid, creatorid, posteddate, year, month, week, day, hour, minute, second, vkgroupid, vktopicid) values (@VkId, @CreatorId, @PostedDate, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @VkGroupId, @VkTopicId) RETURNING id", comment).First();
            }
        }
        public void UpdateComment(TopicComment topicComment)
        {
            if (topicComment == null)
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"update topiccomment set vkid = @VkId, creatorid = @CreatorId, posteddate = @PostedDate, year = @Year, month = @Month, week = @Week, day = @Day, hour = @Hour, minute = @Minute, second = @Second, vkgroupid = @VkGroupId, vktopicid = @VkTopicId where id = @Id", topicComment);
            }
        }
    }
}