namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class MemberSubscription : IVkEntity, IEntity
    {
        public virtual int Id { get; set; }
        public virtual string VkId
        {
            get { return this.SubscribedVkGroupId.ToString(); }
            set { this.SubscribedVkGroupId = int.Parse(value); }
        }

        public virtual long VkMemberId { get; set; }
        public virtual int SubscribedVkGroupId { get; set; }
        public virtual int VkGroupId { get; set; }

        public virtual VkGroupReference SubscribedVkGroup { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{{ Id: {0}, VkGroupId: {1}, VkMemberId: {2}, SubscribedVkGroupId: {3} }}",
                this.Id, 
                this.VkGroupId, 
                this.VkMemberId, 
                this.SubscribedVkGroupId);
        }
    }
}
