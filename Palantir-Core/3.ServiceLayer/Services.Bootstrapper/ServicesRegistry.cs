namespace Ix.Palantir.Services.Bootstrapper
{
    using System;
    using AutoMapper;
    using Framework.ObjectFactory;
    using Ix.Palantir.Services;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Analytics;
    using Ix.Palantir.Services.API.Export;

    using ObjectFactory.StructureMapExtension;

    public class ServicesRegistry : IRegistry
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

            this.InitializeAutoMapper();
        }

        private void InstantiateInStructureMap()
        {
            StructureMap.ObjectFactory.Configure(x =>
            {
                x.For<INavigationService>().Use<NavigationService>();
                x.For<IProjectService>().Use<ProjectService>();
                x.For<IMetricsService>().Use<MetricsService>();
                x.For<IMemberAdvancedSearchService>().Use<MemberAdvancedSearchService>();
                x.For<IUserService>().Use<UserService>();
                x.For<IExportService>().Use<ExportService>();
                x.For<IAnalyticsService>().Use<AnalyticsService>();
                x.For<ISettingsService>().Use<SettingsService>();
                x.For<IConcurrentAnalysisService>().Use<ConcurrentAnalysisService>();
                x.For<IAccountService>().Use<AccountService>();
            });
        }

        private void InitializeAutoMapper()
        {
            Mapper.CreateMap<Ix.Palantir.DomainModel.Project, API.Project>();

            Mapper.CreateMap<Ix.Palantir.Domain.Analytics.API.PostDensity, API.PostDensity>()
                  .ForMember(f => f.DayOfWeek, t => t.MapFrom(s => s.TimeFrame.DayOfWeek))
                  .ForMember(f => f.BeginHour, t => t.MapFrom(s => s.TimeFrame.BeginHour))
                  .ForMember(f => f.EndHour, t => t.MapFrom(s => s.TimeFrame.EndHour));

            Mapper.CreateMap<Ix.Palantir.Services.API.Metrics.MetricsBase, Ix.Palantir.Services.API.Analytics.ConcurrentAnalysisIntroModel>()
                .ForMember(f => f.FirstPostDate, t => t.MapFrom(s => s.FirstPostDate))
                .ForMember(f => f.Project, t => t.MapFrom(s => s.Project))
                .ForMember(f => f.ProjectList, t => t.MapFrom(s => s.ProjectList));
        }
    }
}
