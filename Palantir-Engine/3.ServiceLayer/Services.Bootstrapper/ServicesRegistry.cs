namespace Ix.Palantir.Engine.Services.Bootstrapper
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Engine.Services.API;
    using Ix.Palantir.ObjectFactory.StructureMapExtension;
    using Ix.Palantir.Services;

    public class ServicesRegistry : IRegistry
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
            StructureMap.ObjectFactory.Configure(x => { x.For<ISchedulingService>().Use<SchedulingService>(); });
        }
    }
}