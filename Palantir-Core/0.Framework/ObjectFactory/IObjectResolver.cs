namespace Ix.Framework.ObjectFactory
{
    using System;
    
    public interface IObjectResolver
    {
        T Resolve<T>();
        T ResolveNamed<T>(string instanceName);
        
        object Resolve(Type pluginType);
        object ResolveNamed(Type pluginType, string instanceName);

        void Initialize();
    }
}