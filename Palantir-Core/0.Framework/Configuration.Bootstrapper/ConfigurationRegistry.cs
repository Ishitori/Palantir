namespace Ix.Palantir.Configuration.Bootstrapper
{
    using System;
    using Framework.ObjectFactory;
    using ObjectFactory.StructureMapExtension;

    public class ConfigurationRegistry : IRegistry
    {
        public void InstantiateIn(IObjectResolver objectResolver)
        {
            if (objectResolver is StructureMapObjectResolver)
            {
                this.InstantiateInStructureMap();
            }
            else
            {
                throw new ArgumentException("Unsupported type of object resolver is provided", "objectResolver");
            }
        }
        private void InstantiateInStructureMap()
        {
            StructureMap.ObjectFactory.Configure(x =>
            {
                x.For<Ix.Palantir.Configuration.API.IConfigurationRegistry>().Use<Ix.Palantir.Configuration.ConfigurationRegistry>();
                x.For<Ix.Palantir.Configuration.API.IConfigurationProvider>().Use<Ix.Palantir.Configuration.ConfigurationProvider>();
            });
        }
    }
}
