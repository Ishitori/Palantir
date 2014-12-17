namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class VkEntity : DateEntity, IVkEntity, IEntity
    {
        public virtual int Id { get; set; }
        public virtual int VkGroupId { get; set; }
        public virtual string VkId { get; set; }
    }
}