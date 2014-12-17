namespace Ix.Framework.ObjectFactory
{
    using System;
    using System.Diagnostics;

    [DebuggerStepThrough]
    public static class Factory
    {
        private static IObjectResolver resolver;

        public static T GetInstance<T>()
        {
            return resolver.Resolve<T>();
        }
        public static object GetInstance(Type pluginType)
        {
            return resolver.Resolve(pluginType);
        }
        public static T GetNamed<T>(string instanceName)
        {
            return resolver.ResolveNamed<T>(instanceName);
        }

        public static void SetResolver(IObjectResolver resolverToUse)
        {
            resolver = resolverToUse;
            resolverToUse.Initialize();
        }
    }
}