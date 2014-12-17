namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class Administrator : IEntity
    {
        public virtual int Id { get; set; }
        public virtual long UserId { get; set; }
        public virtual int VkGroupId { get; set; }
    }
}