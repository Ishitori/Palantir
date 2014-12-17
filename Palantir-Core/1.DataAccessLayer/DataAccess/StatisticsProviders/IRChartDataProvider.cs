namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public class IrChartDataProvider : IIrChartDataProvider
    {
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IVkGroupRepository groupRepository;
        private readonly IEntitiesForChartProvider entitiesForChartProvider;
        private readonly IRawDataProvider rawDataProvider;

        public IrChartDataProvider(IDataGatewayProvider dataGatewayProvider, IVkGroupRepository groupRepository, IEntitiesForChartProvider entitiesForChartProvider, IRawDataProvider rawDataProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
            this.groupRepository = groupRepository;
            this.entitiesForChartProvider = entitiesForChartProvider;
            this.rawDataProvider = rawDataProvider;
        }

        public IEnumerable<IEnumerable<PointInTime>> GetInteractionRate(int vkGroupId, DateRange range, Periodicity periodicity)
        {
            var creationDate = this.groupRepository.GetGroupCreationDate(vkGroupId);

            if (!creationDate.HasValue)
            {
                return null;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var posts = dataGateway.GetEntities<Post>()
                               .Where(
                                   x =>
                                   x.VkGroupId == vkGroupId && x.PostedDate >= range.From && x.PostedDate <= range.To)
                               .ToList();
                
                var membersCount = dataGateway.GetEntities<MembersMetaInfo>()
                               .Where(
                                   x =>
                                   x.VkGroupId == vkGroupId && x.PostedDate >= range.From && x.PostedDate <= range.To)
                               .ToList();
                
                var share = new List<TempShareModel>();
                
                var video = dataGateway.GetEntities<Video>()
                               .Where(
                                   x =>
                                   x.VkGroupId == vkGroupId && x.PostedDate >= range.From && x.PostedDate <= range.To)
                               .ToList();
                
                var photo = dataGateway.GetEntities<Photo>()
                               .Where(
                                   x =>
                                   x.VkGroupId == vkGroupId && x.PostedDate >= range.From && x.PostedDate <= range.To)
                               .ToList();

                string str = @"SELECT * FROM membershare WHERE vkgroupid = @groupId AND itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                var postShare = dataGateway.Connection.Query<MemberShare>(str, new { groupId = vkGroupId, from = range.From, to = range.To }).ToList();

                str = @"SELECT * FROM membershare WHERE vkgroupid = @groupId AND itemid = ANY(SELECT vkid::integer FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                var videoShare = dataGateway.Connection.Query<MemberShare>(str, new { groupId = vkGroupId, from = range.From, to = range.To }).ToList();

                str = @"SELECT * FROM membershare WHERE vkgroupid = @groupId AND itemid = ANY(SELECT vkid::integer FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                var photoShare = dataGateway.Connection.Query<MemberShare>(str, new { groupId = vkGroupId, from = range.From, to = range.To }).ToList();

                for (int i = 0; i < posts.Count; i++)
                {
                    Post post = posts[i];
                    var ps = postShare.Where(x => x.ItemId == int.Parse(post.VkId)).ToList();

                    if (ps.Count > 0)
                    {
                        share.AddRange(ps.Select(x => new TempShareModel() { Id = x.Id, PostedDate = post.PostedDate }));
                        post.SharesCount++;
                    }
                }

                for (int i = 0; i < video.Count; i++)
                {
                    var vp = videoShare.Where(x => x.ItemId == int.Parse(video[i].VkId)).ToList();
                    if (vp.Count > 0)
                    {
                        share.AddRange(vp.Select(x => new TempShareModel() { Id = x.Id, PostedDate = video[i].PostedDate }));
                        video[i].ShareCount++;
                    }
                }

                for (int i = 0; i < photo.Count; i++)
                {
                    var phs = photoShare.Where(x => x.ItemId == int.Parse(photo[i].VkId)).ToList();
                    if (phs.Count > 0)
                    {
                        share.AddRange(phs.Select(x => new TempShareModel() { Id = x.Id, PostedDate = photo[i].PostedDate }));
                        photo[i].ShareCount++;
                    }
                }

                int count = membersCount.Count > 0
                                ? membersCount.Last().Count
                                : this.rawDataProvider.GetMembersCount(vkGroupId);

                IEnumerable<PointInTime> ir = this.GetIrEntityForChart(posts, count, range, periodicity);
                IEnumerable<PointInTime> comments = this.entitiesForChartProvider.GetEntitiesForChart(posts, range, periodicity, e => e.Sum(x => x.CommentsCount));
                IEnumerable<PointInTime> likes = this.entitiesForChartProvider.GetEntitiesForChart(posts, range, periodicity, e => e.Sum(x => x.LikesCount));
                IEnumerable<PointInTime> shares = this.entitiesForChartProvider.GetEntitiesForChart(share, range, periodicity, e => e.Count());

                return new List<IEnumerable<PointInTime>> { ir, comments, likes, shares };
            }
        }

        private IEnumerable<PointInTime> GetIrEntityForChart(IEnumerable<Post> postQuery, int members, DateRange range, Periodicity periodicity)
        {
            IList<PointInTime> result = new List<PointInTime>();
            var irCalculator = new InteractionRateCalculator();
            Func<DateTime, DateTime> increase = this.GetTimeIncreaseFunction(periodicity);

            for (var i = range.From; i < range.To; i = increase(i))
            {
                var date = i;
                var posts = postQuery.Where(x => (x.PostedDate >= date) && (x.PostedDate < date.AddMonths(1))).ToList();
                var postcount = posts.Count();
                var commentcount = posts.Sum(x => x.CommentsCount);
                var likecount = posts.Sum(x => x.LikesCount);
                var sharecount = posts.Sum(x => x.SharesCount);
                if (postcount == 0 || members == 0)
                {
                    members = 1;
                    postcount = 1;
                }
                double interactionRate = irCalculator.GetInteractionRate(commentcount, likecount, postcount, sharecount, members);

                result.Add(new PointInTime { Date = date, Value = interactionRate });
            }

            return result;
        }

        private Func<DateTime, DateTime> GetTimeIncreaseFunction(Periodicity periodicity)
        {
            Func<DateTime, DateTime> increase;
            switch (periodicity)
            {
                case Periodicity.ByHour:
                    increase = time => time.AddHours(1);
                    break;

                case Periodicity.ByDayWithHour:
                    increase = time => time.AddDays(1);
                    break;

                case Periodicity.ByDay:
                    increase = time => time.AddDays(1);
                    break;

                case Periodicity.ByWeek:
                    increase = time => time.AddDays(7);
                    break;

                case Periodicity.ByMonth:
                    increase = time => time.AddMonths(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("periodicity");
            }
            return increase;
        }
    }
}