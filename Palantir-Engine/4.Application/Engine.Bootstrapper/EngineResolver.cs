namespace Ix.Palantir.Engine.Bootstrapper
{
    using Caching.Bootstrapper;
    using DataAccess.Bootstrapper;
    using Framework.Bootstrapper;
    using ObjectFactory.StructureMapExtension;
    using Queueing.Bootstrapper;
    using Security.Bootstrapper;

    public class EngineResolver : StructureMapObjectResolver
    {
        public override void Initialize()
        {
            new Ix.Palantir.Configuration.Bootstrapper.ConfigurationRegistry().InstantiateIn(this);
            new CachingRegistry().InstantiateIn(this);
            new FrameworkRegistry().InstantiateIn(this);
            new QueueingRegistry().InstantiateIn(this);
            new DataAccessRegistry().InstantiateIn(this);
            new SecurityRegistry().InstantiateIn(this);
            new Ix.Palantir.Domain.Bootstrapper.DomainRegistry().InstantiateIn(this);
            new Ix.Palantir.Engine.Domain.Bootstrapper.DomainRegistry().InstantiateIn(this);
            new Ix.Palantir.Engine.Services.Bootstrapper.ServicesRegistry().InstantiateIn(this);

            Ix.Palantir.Configuration.ConfigurationRegistry.RegisterConfigurationSection(typeof(Ix.Palantir.Engine.Configuration.JmxClientConfiguration), "jmx-configuration.config");
            Ix.Palantir.Configuration.ConfigurationRegistry.RegisterConfigurationSection(typeof(Ix.Palantir.Engine.Configuration.VkCredentials), "vk-credentials.config");
        }
    }
}