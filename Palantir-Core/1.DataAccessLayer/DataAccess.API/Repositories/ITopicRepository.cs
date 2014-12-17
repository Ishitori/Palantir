namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;

    using Ix.Palantir.DomainModel;

    public interface ITopicRepository
    {
        void Save(Topic topic);
        void SaveComment(TopicComment comment);
        void UpdateComment(TopicComment comment);
        void UpdateTopic(Topic topic);

        Topic GetTopic(int vkGroupId, string vkId);
        IList<Topic> GetTopicsByVkGroupId(int vkGroupId);
        TopicComment GetTopicComment(int vkGroupId, string vkId);
        IList<TopicComment> GetTopicCommentsByVkGroupId(int vkGroupId);
    }
}