namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class Video : VkEntity
    {
        public virtual string Description { get; set; }
        public virtual int Duration { get; set; }
        public virtual string Title { get; set; }
        public virtual int LikesCount { get; set; }
        public virtual int CommentsCount { get; set; }
        public virtual int ShareCount { get; set; }
        public virtual int CommentsAndLikesAndShareSum
        {
            get
            {
                return this.LikesCount + this.CommentsCount + this.ShareCount;
            }
        }
    }
}