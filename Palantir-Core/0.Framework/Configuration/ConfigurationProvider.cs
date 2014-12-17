namespace Ix.Palantir.Configuration
{
    using System.IO;
    using System.Xml.Serialization;
    using Ix.Palantir.Configuration.API;

    public class ConfigurationProvider : IConfigurationProvider
    {
        private static readonly object LockObject = new object();
        private readonly IConfigurationRegistry registry;

        public ConfigurationProvider(IConfigurationRegistry registry)
        {
            this.registry = registry;
        }

        public T GetConfigurationSection<T>()
        {
            if (this.registry.ContainsConfiguration(typeof(T)))
            {
                return (T)this.registry.GetConfiguration(typeof(T));
            }

            lock (LockObject)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                string configurationFilePath = this.registry.GetConfigurationFilePath(typeof(T));
                object configuration;

                using (Stream stream = File.OpenRead(configurationFilePath))
                {
                    configuration = xmlSerializer.Deserialize(stream);
                }

                this.registry.AddConfiguration(typeof(T), configuration);
                return (T)configuration;
            }
        }
    }
}