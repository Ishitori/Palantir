namespace Ix.Palantir.DomainModel
{
    public class MembersDelta : DateEntity, IEntity
    {
        public MembersDelta()
        {
            this.InIds = string.Empty;
            this.OutIds = string.Empty;
        }

        public virtual int Id { get; set; }
        public virtual int VkGroupId { get; set; }
        public virtual string InIds { get; set; }
        public virtual int InCount { get; set; }
        public virtual string OutIds { get; set; }
        public virtual int OutCount { get; set; }
    }
}
