namespace Ix.Palantir.DomainModel
{
    public class MembersMetaInfo : VkEntity, ICounter
    {
        public virtual int Count { get; set; }
    }
}