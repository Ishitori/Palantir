namespace Ix.Palantir.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Analytics;
    using Ix.Palantir.Services.API.Metrics;

    public class ConcurrentAnalysisService : IConcurrentAnalysisService
    {
        private readonly IMetricsService metricsService;
        private readonly IProjectService projectService;
        private readonly IProjectRepository projectRepository;
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IUserStatusCalculator usersCalculator;
        private readonly IValueRanker ranker;

        public ConcurrentAnalysisService(IMetricsService metricsService, IProjectService projectService, IProjectRepository projectRepository, IUnitOfWorkProvider unitOfWorkProvider, IUserStatusCalculator usersCalculator, IValueRanker ranker)
        {
            this.metricsService = metricsService;
            this.projectService = projectService;
            this.projectRepository = projectRepository;
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.usersCalculator = usersCalculator;
            this.ranker = ranker;
        }

        public ConcurrentAnalysisIntroModel GetConcurrentAnalysisIntro(int projectId)
        {
            var baseMetrics = this.metricsService.GetBaseMetrics(projectId);
            var model = AutoMapper.Mapper.Map<ConcurrentAnalysisIntroModel>(baseMetrics);
            model.ConcurrentIds = this.projectService.GetConcurentsIdsOf(projectId);

            return model;
        }
        public void UpdateProjectsConcurrents(int projectId, IList<int> concurrentIds)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                this.projectRepository.DeleteAllConcurrents(projectId);
                this.projectRepository.AddConcurents(projectId, concurrentIds);
            }
        }
        public ConcurrentAnalysisModel GetConcurrentAnalysis(int projectId, IList<int> concurrentIds, DateRange dateRange)
        {
            IDictionary<int, IList<long>> usersIntersection;
            DashboardMetrics initialMetrics = this.metricsService.GetDashboardMetrics(projectId, dateRange);
            RankedModel initialRankedModel = RankedModel.Create(initialMetrics);
            IList<DashboardMetrics> concurrentsMetrics = new List<DashboardMetrics>();
            IList<RankedModel> concurrentsRankedMetrics = new List<RankedModel>();

            foreach (var concurrentId in concurrentIds)
            {
                var concurrentMetrics = this.metricsService.GetDashboardMetrics(concurrentId, dateRange);
                var rankedModel = RankedModel.Create(concurrentMetrics);
                concurrentsMetrics.Add(concurrentMetrics);
                concurrentsRankedMetrics.Add(rankedModel);
            }

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                usersIntersection = this.usersCalculator.FindUsersIntersection(initialMetrics.AllUserIds, concurrentsMetrics.ToDictionary(c => c.Project.Id, v => v.AllUserIds));
            }

            this.AssignUsersIntersection(concurrentsRankedMetrics, usersIntersection);
            this.RankModels(initialRankedModel, concurrentsRankedMetrics);

            var result = new ConcurrentAnalysisModel(initialRankedModel, concurrentsRankedMetrics);
            return result;
        }

        private void AssignUsersIntersection(IList<RankedModel> concurrentsRankedMetrics, IDictionary<int, IList<long>> usersIntersection)
        {
            foreach (var model in concurrentsRankedMetrics)
            {
                if (usersIntersection.ContainsKey(model.ProjectId))
                {
                    model.SameUsersCount = usersIntersection[model.ProjectId].Count;
                }
            }
        }

        private void RankModels(RankedModel initial, IList<RankedModel> others)
        {
            this.ranker.RankValues(initial.AverageCommentsPerPost, others.Select(o => o.AverageCommentsPerPost));
            this.ranker.RankValues(initial.AverageLikesPerPost, others.Select(o => o.AverageLikesPerPost));
            this.ranker.RankValues(initial.InteractionRate, others.Select(o => o.InteractionRate));
            this.ranker.RankValues(initial.ResponseRate, others.Select(o => o.ResponseRate));
            this.ranker.RankValues(initial.InvolmentRate, others.Select(o => o.InvolmentRate));
            this.ranker.RankValues(initial.UgcRate, others.Select(o => o.UgcRate));

            this.ranker.RankValues(initial.PhotosCount, others.Select(o => o.PhotosCount));
            this.ranker.RankValues(initial.VideosCount, others.Select(o => o.VideosCount));
            this.ranker.RankValues(initial.PostsCount, others.Select(o => o.PostsCount));
            this.ranker.RankValues(initial.TopicsCount, others.Select(o => o.TopicsCount));
            this.ranker.RankValues(initial.TopicCommentsCount, others.Select(o => o.TopicCommentsCount));
            this.ranker.RankValues(initial.SharePerPost, others.Select(o => o.SharePerPost));
          
            this.ranker.RankValues(initial.UsersPostCount, others.Select(o => o.UsersPostCount));
            this.ranker.RankValues(initial.AdminPostCount, others.Select(o => o.AdminPostCount));

            this.ranker.RankValues(initial.UsersPostsPerUser, others.Select(o => o.UsersPostsPerUser));
            this.ranker.RankValues(initial.AdminPostsPerAdmin, others.Select(o => o.AdminPostsPerAdmin));

            this.ranker.RankValues(initial.UsersCount, others.Select(o => o.UsersCount));
            this.ranker.RankValues(initial.BotsCount, others.Select(o => o.BotsCount));
            this.ranker.RankValues(initial.InactiveUsersCount, others.Select(o => o.InactiveUsersCount));
            this.ranker.RankValues(initial.ActiveUsersCount, others.Select(o => o.ActiveUsersCount));
        }
    }
}