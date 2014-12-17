namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class VideoComment : VkEntity
    {
        public virtual string VkVideoId { get; set; }
        public virtual long CreatorId { get; set; }
    }
}