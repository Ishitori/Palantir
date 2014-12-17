namespace Ix.Palantir.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API.Analytics;

    using PostDensity = Ix.Palantir.Services.API.PostDensity;

    public class AnalyticsService : IAnalyticsService
    {
        private readonly IRawDataProvider rawDataProvider;
        private readonly IPostDensityCalculator postDensityCalculator;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProjectRepository projectRepository;
        private readonly IUserStatusCalculator usersCalculator;

        public AnalyticsService(IRawDataProvider rawDataProvider, IPostDensityCalculator postDensityCalculator, IDateTimeHelper dateTimeHelper, IProjectRepository projectRepository, IUserStatusCalculator usersCalculator)
        {
            this.rawDataProvider = rawDataProvider;
            this.postDensityCalculator = postDensityCalculator;
            this.dateTimeHelper = dateTimeHelper;
            this.projectRepository = projectRepository;
            this.usersCalculator = usersCalculator;
        }

        public PostDensity GetPostMostCrowdedTime(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            IList<Post> posts = this.rawDataProvider.GetPosts(vkGroup.Id, dateRange);
            IList<Domain.Analytics.API.PostDensity> mostCrowdedTime = this.postDensityCalculator.GetMostCrowdedTime(posts);
            var mostCrowdedDayOfWeek = mostCrowdedTime.Count > 0 ? mostCrowdedTime[0] : new Domain.Analytics.API.PostDensity();
            mostCrowdedDayOfWeek.TimeFrame = this.dateTimeHelper.GetLocalUserTimeFrame(mostCrowdedDayOfWeek.TimeFrame);

            return AutoMapper.Mapper.Map<PostDensity>(mostCrowdedDayOfWeek);
        }

        public UserStatistics GetInactiveUsersCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            IList<long> postCreatorIds = this.rawDataProvider.GetPostCreatorIds(vkGroup.Id, dateRange);
            IList<long> postCommentCreatorIds = this.rawDataProvider.GetPostCommentCreatorIds(vkGroup.Id, dateRange);
            IList<long> topicCreatorIds = this.rawDataProvider.GetTopicCreatorIds(vkGroup.Id, dateRange);
            IList<long> topicCommentCreatorIds = this.rawDataProvider.GetTopicCommentCreatorIds(vkGroup.Id, dateRange);
            IList<MemberWithStatus> userIds = this.rawDataProvider.GetUserIdsWithStatus(vkGroup.Id);

            var userStatistics = new UserStatistics();
            userStatistics.AllUsers = userIds.Select(u => u.MemberId).ToList();
            userStatistics.InactiveUsers = this.usersCalculator.GetInactiveUsers(userIds, postCreatorIds, postCommentCreatorIds, topicCreatorIds, topicCommentCreatorIds);
            userStatistics.Bots = this.usersCalculator.GetBots(userIds);
            userStatistics.DeletedUsers = this.usersCalculator.GetDeletedUsers(userIds);
            userStatistics.BlockedUsers = this.usersCalculator.GetBlockedUsers(userIds);
            userStatistics.ActiveUsers = this.usersCalculator.GetActiveUsers(userStatistics.AllUsers, userStatistics.InactiveUsers, userStatistics.Bots, userStatistics.DeletedUsers, userStatistics.BlockedUsers);

            return userStatistics;
        }
    }
}