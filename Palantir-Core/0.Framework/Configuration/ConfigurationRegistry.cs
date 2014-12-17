namespace Ix.Palantir.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Web;
    using System.Web.Caching;
    using Ix.Palantir.Configuration.API;
    using Queueing;

    public class ConfigurationRegistry : IConfigurationRegistry
    {
        private static readonly object LockObject;
        private static readonly IDictionary<Type, string> configurationFilePaths;

        static ConfigurationRegistry()
        {
            LockObject = new object();
            configurationFilePaths = new Dictionary<Type, string>();
            configurationFilePaths.Add(typeof(FeedProcessingConfig), "feed-processing.config");
            configurationFilePaths.Add(typeof(QueueingConfig), "queueing.config");
            configurationFilePaths.Add(typeof(CachingConfig), "caching.config");
        }

        public static void RegisterConfigurationSection(Type configurationType, string fileName)
        {
            Contract.Requires(configurationType != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(fileName));

            lock (LockObject)
            {
                configurationFilePaths.Add(configurationType, fileName); 
            }
        }

        public object GetConfiguration(Type type)
        {
            if (type.AssemblyQualifiedName != null)
            {
                var config = HttpRuntime.Cache.Get(type.AssemblyQualifiedName);
                return config;
            }

            return null;
        }
        public void AddConfiguration(Type type, object configuration)
        {
            if (type.AssemblyQualifiedName != null)
            {
                HttpRuntime.Cache.Insert(type.AssemblyQualifiedName, configuration, new CacheDependency(this.GetConfigurationFilePath(type)));
            }
        }

        public string GetConfigurationFilePath(Type type)
        {
            lock (LockObject)
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationFilePaths[type]);
            }
        }
        public bool ContainsConfiguration(Type type)
        {
            var config = this.GetConfiguration(type);
            return config != null;
        }
    }
}