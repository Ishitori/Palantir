namespace Ix.Palantir.Configuration.API
{
    public interface IConfigurationProvider
    {
        T GetConfigurationSection<T>();
    }
}