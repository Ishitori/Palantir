namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class TopicComment : VkEntity
    {
        public virtual string VkTopicId { get; set; }
        public virtual long CreatorId { get; set; }
    }
}