namespace Ix.Palantir.Framework.Bootstrapper
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Configuration;
    using Ix.Palantir.FileSystem;
    using Ix.Palantir.FileSystem.API;
    using Ix.Palantir.Localization;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.LockServer.API;
    using Logging;
    using ObjectFactory.StructureMapExtension;
    using StructureMap;
    using StructureMap.Pipeline;
    using Utilities;

    public class FrameworkRegistry : IRegistry
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
                x.For<ILog>().Use(a => LogManager.GetLogger());
                x.For<IWebPageDownloader>().Use<WebPageDownloader>();
                x.For<IDateTimeHelper>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<DateTimeHelper>();

                ConfigurationRegistry.RegisterConfigurationSection(typeof(FileSystemConfiguration), "file-system.config");

                x.For<IFileSystemFactory>().Use<FileSystemFactory>();
                x.For<IFileSystem>().Use<FileSystem>();
                x.For<IWebUtilities>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<WebUtilities>();
                x.For<ILockServer>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<LockServer.LockServer>();
            });
        }  
    }
}
