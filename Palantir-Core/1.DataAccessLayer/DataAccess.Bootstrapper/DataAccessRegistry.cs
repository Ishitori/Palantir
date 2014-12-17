namespace Ix.Palantir.DataAccess.Bootstrapper
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Caching;
    using Ix.Palantir.Configuration;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Export;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DataAccess.Export;
    using Ix.Palantir.DataAccess.NHibernateImpl;
    using Ix.Palantir.DataAccess.Repositories;
    using Ix.Palantir.DataAccess.Repositories.CachingWrapper;
    using Ix.Palantir.DataAccess.StatisticsProviders;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.LockServer.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.ObjectFactory.StructureMapExtension;
    using StructureMap;
    using StructureMap.Pipeline;

    public class DataAccessRegistry : IRegistry
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
                x.For<IQueryBuilder>().Use<QueryBuilder>();
                x.For<ITransactionScope>().Use<TransactionScope>();
                x.For<IUnitOfWork>().Use<UnitOfWork>();
                x.For<IUnitOfWorkProvider>().LifecycleIs(new SingletonLifecycle()).Use<UnitOfWorkProvider>();
                x.For<IDataGateway>().Use(a => new SessionFactory().CreateSession());
                x.For<IDurableDataGateway>().Use(a => new SessionFactory().CreateDurableSession());
                x.For<IDataGatewayProvider>().Use<DataGatewayProvider>();
                x.For<IPersistentDataGatewayProvider>().Use<DataGatewayProvider>();

                x.For<IEntitiesForChartProvider>().Use<EntitiesForChartProvider>();
                x.For<IIrChartDataProvider>().Use<IrChartDataProvider>();
                x.For<IStatisticsProvider>().Use<StatisticsProvider>();
                x.For<IMemberAdvancedSearcher>().Use<MemberAdvancedSearcher>();
                x.For<IKpiProvider>().Use<KpiProvider>();
                x.For<IRawDataProvider>().Use<RawDataProvider>();
                x.For<IFeedProcessingCachingStrategy>().Use<FeedProcessingCachingStrategy>();
                x.For<IExportDataProvider>().Use<ExportDataProvider>();
                x.For<IMembersDeltaUpdater>().Use<MembersDeltaUpdater>();
                x.For<IMemberAdvancedSearchCache>().Use<MemberAdvancedSearchCache>();

                this.ConfigureRepositories(x);
            });
        }

        private void ConfigureRepositories(ConfigurationExpression x)
        {
            x.For<IVkGroupRepository>().Use<VkGroupRepository>();
            x.For<IMemberRepository>().Use<MemberRepository>();

            var cachingConfig = Factory.GetInstance<IConfigurationProvider>().GetConfigurationSection<CachingConfig>();

            if (!cachingConfig.IsCachingEnabledFor(typeof(IPostRepository)))
            {
                x.For<IPostRepository>().Use<PostRepository>();
            }
            else
            {
                x.For<IPostRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new PostRepositoryCachingWrapper(new PostRepository(c.GetInstance<IDataGatewayProvider>()), c.GetInstance<IDataGatewayProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(IPostRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>())));
            }

            if (!cachingConfig.IsCachingEnabledFor(typeof(ITopicRepository)))
            {
                x.For<ITopicRepository>().Use<TopicRepository>();
            }
            else
            {
                x.For<ITopicRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new TopicRepositoryCachingWrapper(new TopicRepository(c.GetInstance<IDataGatewayProvider>()), c.GetInstance<IDataGatewayProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(ITopicRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>())));
            }

            x.For<IProjectRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Hybrid)).Use(c => new ProjectRepositoryCachingWrapper(new ProjectRepository(c.GetInstance<IDataGatewayProvider>())));
            /*if (!cachingConfig.IsCachingEnabledFor(typeof(IProjectRepository)))
            {
                x.For<IProjectRepository>().Use<ProjectRepository>();
            }
            else
            {
            }*/

            if (!cachingConfig.IsCachingEnabledFor(typeof(IPhotoRepository)))
            {
                x.For<IPhotoRepository>().Use<PhotoRepository>();
            }
            else
            {
                x.For<IPhotoRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new PhotoRepositoryCachingWrapper(new PhotoRepository(c.GetInstance<IDataGatewayProvider>()), c.GetInstance<IDataGatewayProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(IPhotoRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>()), c.GetInstance<ILog>()));
            }

            if (!cachingConfig.IsCachingEnabledFor(typeof(IVideoRepository)))
            {
                x.For<IVideoRepository>().Use<VideoRepository>();
            }
            else
            {
                x.For<IVideoRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new VideoRepositoryCachingWrapper(new VideoRepository(c.GetInstance<IDataGatewayProvider>()), c.GetInstance<IDataGatewayProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(IVideoRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>())));
            }

            if (!cachingConfig.IsCachingEnabledFor(typeof(IMemberRepository)))
            {
                x.For<IMemberRepository>().Use<MemberRepository>();
            }
            else
            {
                x.For<IMemberRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new MemberRepositoryCachingWrapper(new MemberRepository(c.GetInstance<IDataGatewayProvider>()), c.GetInstance<IRawDataProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(IMemberRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>())));
            }

            if (!cachingConfig.IsCachingEnabledFor(typeof(IMemberLikeSharesRepository)))
            {
                x.For<IMemberLikeSharesRepository>().Use<MemberLikeSharesRepository>();
            }
            else
            {
                x.For<IMemberLikeSharesRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new MemberLikeSharesRepositoryCachingWrapper(new MemberLikeSharesRepository(c.GetInstance<IDataGatewayProvider>()), c.GetInstance<IRawDataProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(IMemberLikeSharesRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>()), c.GetInstance<ILog>()));
            }

            if (!cachingConfig.IsCachingEnabledFor(typeof(IMemberSubscriptionRepository)))
            {
                x.For<IMemberSubscriptionRepository>().Use<MemberSubscriptionRepository>();
            }
            else
            {
                x.For<IMemberSubscriptionRepository>().LifecycleIs(Lifecycles.GetLifecycle(InstanceScope.Singleton)).Use(c => new MemberSubscriptionRepositoryCachingWrapper(new MemberSubscriptionRepository(c.GetInstance<IDataGatewayProvider>(), c.GetInstance<ILog>()), c.GetInstance<IDataGatewayProvider>(), new FeedProcessingCachingStrategy(c.GetInstance<ILockServer>(), c.GetInstance<CachingHelper>(), cachingConfig.GetCachingSettings(typeof(IMemberSubscriptionRepository)), c.GetInstance<IVkGroupRepository>(), c.GetInstance<IConfigurationProvider>(), c.GetInstance<IDateTimeHelper>(), c.GetInstance<ICacheStorage>()), c.GetInstance<ILog>()));
            }

            x.For<IFeedRepository>().Use<FeedRepository>();
            x.For<IPlaceRepository>().Use<PlaceRepository>();
            x.For<IListRepository>().Use<ListRepository>();
        }
    }
}