namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class Photo : VkEntity
    {
        public virtual string Text
        {
            get; set;
        }
        public virtual string AlbumId
        {
            get;
            set;
        }
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