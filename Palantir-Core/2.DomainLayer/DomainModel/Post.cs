namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class Post : VkEntity
    {
        public virtual long CreatorId { get; set; }
        public virtual string Text { get; set; }
        public virtual int LikesCount { get; set; }
        public virtual int CommentsCount { get; set; }
        public virtual int SharesCount { get; set; }
    }
}