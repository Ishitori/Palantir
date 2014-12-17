namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class VkGroup : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string Url { get; set; }
        public virtual string Name { get; set; }
        public virtual VkGroupType Type { get; set; }
    }
}