namespace Ix.Palantir.Engine.Domain.Bootstrapper
{
    using System;

    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Engine.Configuration;
    using Ix.Palantir.Infrastructure.Process;
    using Ix.Palantir.ObjectFactory.StructureMapExtension;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.UrlManagement;
    using Ix.Palantir.UrlManagement.API;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;
    using Ix.Palantir.Vkontakte.Workflows.Providers;
    using Ix.Palantir.Vkontakte.Workflows.VkMappers;

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
                x.For<IProcessorFactory>().Use<ProcessorFactory>();
                x.For<IGroupInfoProvider>().Use<GroupInfoProvider>();

                x.For<WallPostFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<WallPostFeedProcessor>();
                x.For<WallPostCommentFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<WallPostCommentFeedProcessor>();
                x.For<PhotoFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<PhotoFeedProcessor>();
                x.For<VideoFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VideoFeedProcessor>();
                x.For<VideoCommentFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VideoCommentFeedProcessor>();
                x.For<VideoLikesFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VideoLikesFeedProcessor>();
                x.For<TopicFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<TopicFeedProcessor>();
                x.For<TopicCommentFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<TopicCommentFeedProcessor>();
                x.For<MembersCountFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<MembersCountFeedProcessor>();
                x.For<MembersFeedProcessor>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<MembersFeedProcessor>();

                x.For<IVkUrlProvider>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<VkUrlProvider>();
                x.For<IEntityIdBuilder>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use<EntityIdBuilder>();
                x.For<IExportDataProcess>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<ExportDataProcess>();

                x.For<IVkResponseMapper>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VkResponseMapper>();
                x.For<IMemberMapper>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<MemberMapper>();
                x.For<IVkDataLimits>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VkDataLimits>();
                x.For<IVkCommandExecuter>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VkCommandExecuter>();
                x.For<IVkConnectionBuilder>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VkConnectionBuilder>();
                x.For<IVkDataProvider>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use<VkDataProvider>();
                x.For<IProcessingStrategy>().Use<ProcessingStrategy>();
                x.For<IProvidingStrategy>().Use<ProvidingStrategy>();
                x.For<IMemberVersionProvider>().Use<MemberVersionProvider>();

                x.For<JmxClientConfiguration>().Use(() => Factory.GetInstance<IConfigurationProvider>().GetConfigurationSection<JmxClientConfiguration>());
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
        }
    }
}