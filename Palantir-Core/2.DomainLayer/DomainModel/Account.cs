namespace Ix.Palantir.DomainModel
{
    using System;
    using Ix.Palantir.Security.API;

    [Serializable]
    public class Account : IAccount
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual int? MaxProjectsCount { get; set; }
        public virtual bool CanDeleteProjects { get; set; }
    }
}