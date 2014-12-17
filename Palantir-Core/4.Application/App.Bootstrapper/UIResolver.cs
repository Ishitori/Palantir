namespace Ix.Palantir.App.Bootstrapper
{
    using Caching.Bootstrapper;
    using Configuration.Bootstrapper;
    using DataAccess.Bootstrapper;
    using Domain.Bootstrapper;
    using Framework.Bootstrapper;
    using ObjectFactory.StructureMapExtension;
    using Queueing.Bootstrapper;
    using Security.Bootstrapper;
    using Services.Bootstrapper;

    public class AppResolver : StructureMapObjectResolver
    {
        public override void Initialize()
        {
            new ConfigurationRegistry().InstantiateIn(this);
            new CachingRegistry().InstantiateIn(this);
            new FrameworkRegistry().InstantiateIn(this);
            new QueueingRegistry().InstantiateIn(this);
            new DataAccessRegistry().InstantiateIn(this);
            new SecurityRegistry().InstantiateIn(this);
            new DomainRegistry().InstantiateIn(this);
            new ServicesRegistry().InstantiateIn(this);
        }
    }
}