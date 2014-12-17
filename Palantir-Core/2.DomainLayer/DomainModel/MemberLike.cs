namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class MemberLike : IVkEntity, IEntity
    {
        public virtual int Id { get; set; }
        public virtual string VkId
        {
            get
            {
                return this.VkMemberId.ToString();
            }
            set
            {
                this.VkMemberId = int.Parse(value);
            }
        }
        public virtual int VkGroupId { get; set; }

        public virtual int ItemId { get; set; }
        public virtual long VkMemberId
        {
            get;
            set;
        }
        public virtual LikeShareType ItemType { get; set; }
    }
}