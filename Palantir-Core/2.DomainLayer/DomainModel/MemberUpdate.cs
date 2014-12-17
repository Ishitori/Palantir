namespace Ix.Palantir.DomainModel
{
    using System;

    public class MemberUpdate
    {
        public virtual int Id { get; set; }
        public virtual int VkGroupId { get; set; }
        public virtual long VkMemberId { get; set; }
        public virtual bool IsNew { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}