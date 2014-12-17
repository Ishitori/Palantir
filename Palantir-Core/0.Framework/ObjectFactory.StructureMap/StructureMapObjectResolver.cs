namespace Ix.Palantir.ObjectFactory.StructureMapExtension
{
    using System;
    using System.Diagnostics;
    using Ix.Framework.ObjectFactory;

    [DebuggerStepThrough]
    public abstract class StructureMapObjectResolver : IObjectResolver
    {
        public T Resolve<T>()
        {
            return StructureMap.ObjectFactory.GetInstance<T>();
        }
        public object Resolve(Type pluginType)
        {
            return StructureMap.ObjectFactory.GetInstance(pluginType);
        }
        public T ResolveNamed<T>(string instanceName)
        {
            return StructureMap.ObjectFactory.GetNamedInstance<T>(instanceName);
        }
        public object ResolveNamed(Type pluginType, string instanceName)
        {
            return StructureMap.ObjectFactory.GetNamedInstance(pluginType, instanceName);
        }
        public abstract void Initialize();
    }
}