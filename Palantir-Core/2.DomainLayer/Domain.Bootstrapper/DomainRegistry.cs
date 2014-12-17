namespace Ix.Palantir.Domain.Bootstrapper
{
    using System;
    using DomainModel;
    using Framework.ObjectFactory;

    using Ix.Palantir.Domain.Analytics;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.UrlManagement;
    using Ix.Palantir.UrlManagement.API;
    using ObjectFactory.StructureMapExtension;

    using StructureMap;
    using StructureMap.Pipeline;

    public class DomainRegistry : IRegistry
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
                x.For<IVkUrlProvider>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<VkUrlProvider>();
                x.For<IEntityIdBuilder>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<EntityIdBuilder>();

                x.For<IPostDensityCalculator>().Singleton().Use<PostDensityCalculator>();
                x.For<IUserStatusCalculator>().Singleton().Use<UserStatusCalculator>();
                x.For<IValueRanker>().Singleton().Use<ValueRanker>();
            });

            this.ConfigureAssembly();
        }

        private void ConfigureAssembly()
        {
            var commandRepository = Factory.GetInstance<ICommandRepository>();
            commandRepository.RegisterCommand(new FeedQueueItem());
            commandRepository.RegisterCommand(new DataFeed());
            commandRepository.RegisterCommand(new GroupQueueItem(0));
            commandRepository.RegisterCommand(new ExportReportCommand());
            commandRepository.RegisterCommand(new ExportResultCommand());
            commandRepository.RegisterCommand(new CreateProjectCommand());
            commandRepository.RegisterCommand(new CreateProjectResultCommand());
            commandRepository.RegisterCommand(new DeleteProjectCommand());
            commandRepository.RegisterCommand(new UpdateMembersDeltaCommand());
        }
    }
}