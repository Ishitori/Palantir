namespace Ix.Palantir.Configuration.API
{
    using System;

    public interface IConfigurationRegistry
    {
        object GetConfiguration(Type type);
        void AddConfiguration(Type type, object configuration);

        string GetConfigurationFilePath(Type type);
        bool ContainsConfiguration(Type type);
    }
}