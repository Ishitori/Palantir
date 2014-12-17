namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class PostComment : VkEntity
    {
        public virtual long CreatorId { get; set; }
        public virtual long? ReplyToUserId { get; set; }
        public virtual int? ReplyToVkId { get; set; }
        public virtual string VkPostId { get; set; }
    }
}