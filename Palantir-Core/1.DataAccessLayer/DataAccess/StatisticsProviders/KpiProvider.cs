namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public class KpiProvider : IKpiProvider
    {
        private readonly IVkGroupRepository groupRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IProjectRepository projectRepository;

        public KpiProvider(IVkGroupRepository groupRepository, IDataGatewayProvider dataGatewayProvider, IProjectRepository projectRepository)
        {
            this.groupRepository = groupRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.projectRepository = projectRepository;
        }

        public Kpi GetKpis(int projectId, DateRange dateRange)
        {
            VkGroup vkGroup = this.projectRepository.GetVkGroup(projectId);
            var query = new KpiProviderQueries(dateRange);
            IList<long> adminIds = this.groupRepository.GetAdministratorIds(vkGroup.Id, true);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                object parameters = new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To, adminids = adminIds };
                dynamic postQueryResult = dataGateway.Connection.Query<dynamic>(query.PostDataQuery, parameters).SingleOrDefault();
                int? membersQueryResult = dataGateway.Connection.Query<int?>(query.MemberCountQuery, parameters).FirstOrDefault();

                if (!membersQueryResult.HasValue)
                {
                    membersQueryResult = dataGateway.Connection.Query<int?>(query.GenericMemberCountQuery, parameters).SingleOrDefault();
                }

                int postsWithAdminCommentsCount = dataGateway.Connection.Query<int>(query.PostsWithAdminCommentsCountQuery, parameters).SingleOrDefault();
                int adminPostsCount = dataGateway.Connection.Query<int>(query.AdminPostsCountQuery, parameters).SingleOrDefault();

                string str = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                var postShare = dataGateway.Connection.Query<int>(str, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).FirstOrDefault();

                StatisticsCalculator calculator = new StatisticsCalculator(
                    postQueryResult.postscount ?? 0,
                    postQueryResult.likescount ?? 0,
                    postQueryResult.commentscount ?? 0,
                    postShare,
                    adminPostsCount,
                    postsWithAdminCommentsCount,
                    membersQueryResult.Value);

                Kpi kpi = calculator.CalculateKpis();

                kpi.UserPostsPerUser = this.GetPostsForAnalogPeriod(dataGateway, vkGroup.Id, dateRange, x => !adminIds.Contains(x.CreatorId));
                kpi.AdminPostsPerAdmin = this.GetPostsForAnalogPeriod(dataGateway, vkGroup.Id, dateRange, x => adminIds.Contains(x.CreatorId));

                return kpi;
            }
        }

        public double GetAverageLikesPerPost(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = dateRange.IsSpecified
                    ? @"select count(*)::int as Item, sum(likescount)::int as Value from post where vkgroupid = @vkgroupid"
                    : @"select count(*)::int as Item, sum(likescount)::int as Value from post where vkgroupid = @vkgroupid and posteddate >= @from and posteddate <= @to";
                GroupedObject<int> stat = dataGateway.Connection.Query<GroupedObject<int>>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                return stat.Item > 0 && stat.Value > 0 ? (double)stat.Item / stat.Value : 0;
            }
        }
        public double GetInteractionRate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            var creationDate = this.groupRepository.GetGroupCreationDate(vkGroup.Id);

            if (!creationDate.HasValue)
            {
                return 0;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = dateRange.IsSpecified
                    ? @"select count(*)::int as PostsCount, sum(likescount)::int as LikesCount, sum(commentscount)::int as CommentsCount from post where vkgroupid = @vkgroupid"
                    : @"select count(*)::int as PostsCount, sum(likescount)::int as LikesCount, sum(commentscount)::int as CommentsCount from post where vkgroupid = @vkgroupid and posteddate >= @from and posteddate <= @to";

                string memberCountQuery = @"select count(*)::int from member where vkgroupid = @vkgroupid";

                dynamic stat = dataGateway.Connection.Query<dynamic>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                int memberCount = dataGateway.Connection.Query<int>(memberCountQuery, new { vkgroupid = vkGroup.Id }).SingleOrDefault();

                return stat.postscount > 0 && memberCount > 0
                    ? (stat.likescount + stat.commentscount) / (double)stat.postscount 
                    : 0;
            }
        }

        public double GetResponseRate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            IList<long> adminIds = this.groupRepository.GetAdministratorIds(vkGroup.Id, true);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                const string Query = @"
select count(distinct pc.vkpostid) from postcomment pc
where 
	    pc.vkgroupid = @vkgroupid
	AND pc.creatorid in @adminids
	AND pc.vkpostid in (select p.vkid from post p where p.vkgroupid = @vkgroupid AND p.creatorid not in @adminids)
";
                double postsCount = dataGateway.GetEntities<Post>().Count(x => x.VkGroupId == vkGroup.Id);
                long postsWithAdminCommentsCount = dataGateway.Connection.Query<long>(Query, new { vkgroupid = vkGroup.Id, adminids = adminIds }).Single();

                return postsCount > 0 ? postsWithAdminCommentsCount / postsCount : 0;
            }
        }

        public double GetInvolmentRate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                double postsCount = dataGateway.GetEntities<Post>().Count(x => x.VkGroupId == vkGroup.Id);
                var usersInfo = dataGateway.GetEntities<MembersMetaInfo>().OrderByDescending(u => u.Id).Take(1).SingleOrDefault();

                return usersInfo != null && usersInfo.Count > 0 ? postsCount / usersInfo.Count : 0;
            }
        }

        public double GetUgcRate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            IList<long> adminIds = this.groupRepository.GetAdministratorIds(vkGroup.Id, true);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                double postsCount = dataGateway.GetEntities<Post>().Count(x => x.VkGroupId == vkGroup.Id);
                int adminPostsCount = dataGateway.GetEntities<Post>().Count(x => x.VkGroupId == vkGroup.Id && adminIds.Contains(x.CreatorId));
                double userPosts = postsCount - adminPostsCount;

                return postsCount > 0 ? userPosts / postsCount : 0;
            }
        }

        private double GetPostsForAnalogPeriod(IDataGateway dataGateway, int vkGroupId, DateRange range, Expression<Func<Post, bool>> additionalCondition)
        {
            var query = dataGateway.GetEntities<Post>().Where(x => x.VkGroupId == vkGroupId && x.PostedDate.Date < DateTime.Now.Date);
            var anyPost = query.OrderBy(x => x.PostedDate).FirstOrDefault();
            if (anyPost != null)
            {
                var firstPostDate = anyPost.PostedDate.Date;
                var totalPosts = query.Count(additionalCondition);
                var days = (DateTime.Now.Date - firstPostDate).Days;
                if (days >= range.DaysInRange)
                {
                    var postsPerDay = ((double)totalPosts / days) * range.DaysInRange;
                    return postsPerDay;
                }

                // N/A
                return -1;
            }
            return 0;
        }
    }
}