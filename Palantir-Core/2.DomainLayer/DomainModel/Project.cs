namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class Project : IEntity
    {
        public virtual int AccountId { get; set; }
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime CreationDate { get; set; }

        public virtual VkGroup VkGroup { get; set; }
    }
}