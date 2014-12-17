namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class City : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string VkId { get; set; }
        public virtual string Title { get; set; }
    }
}