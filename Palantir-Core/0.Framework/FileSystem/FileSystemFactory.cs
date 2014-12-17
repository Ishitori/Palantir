namespace Ix.Palantir.FileSystem
{
    using System.IO;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.FileSystem.API;

    public class FileSystemFactory : IFileSystemFactory
    {
        private readonly IConfigurationProvider configurationProvider;

        public FileSystemFactory(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IFileSystem CreateFileSystem(int userId)
        {
            var configuration = this.configurationProvider.GetConfigurationSection<FileSystemConfiguration>();

            if (!Directory.Exists(configuration.RootDir))
            {
                Directory.CreateDirectory(configuration.RootDir);
            }

            string userDirectory = Path.Combine(configuration.RootDir, userId.ToString());

            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }

            return new FileSystem(userId, configuration);
        }
    }
}