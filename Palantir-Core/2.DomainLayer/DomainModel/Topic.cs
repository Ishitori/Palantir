namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class Topic : VkEntity
    {
        public virtual string Title { get; set; }
        public virtual long CreatedByVkId { get; set; }
        public virtual int CommentsCount { get; set; }
        public virtual DateTime LastCommentDate { get; set; }
    }
}