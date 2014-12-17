namespace Caching.Bootstrapper
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Caching;
    using Ix.Palantir.ObjectFactory.StructureMapExtension;
    using StructureMap;
    using StructureMap.Pipeline;

    public class CachingRegistry : IRegistry
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
                x.For<ICacheStorage>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<CacheStorage>();
            });
        }
    }
}