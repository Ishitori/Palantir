namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DataAccess.StatisticsProviders.EmptingsStrategy;
    using Ix.Palantir.DataAccess.StatisticsProviders.Helpers;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public class StatisticsProvider : IStatisticsProvider
    {
        private const string CONST_DateRangeSuffix = " and posteddate >= @from and posteddate <= @to";

        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IProjectRepository projectRepository;
        private readonly IEntitiesForChartProvider entitiesForChartProvider;
        private readonly BirthdayGrouper birthdayGrouper;

        public StatisticsProvider(IDataGatewayProvider dataGatewayProvider, IProjectRepository projectRepository, IEntitiesForChartProvider entitiesForChartProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
            this.projectRepository = projectRepository;
            this.entitiesForChartProvider = entitiesForChartProvider;
            this.birthdayGrouper = new BirthdayGrouper();
        }

        public DateTime? GetLastMemberDate(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                DateTime? lastPostDate = dataGateway.Connection.Query<DateTime?>("select max(posteddate) from membersmetainfo where vkgroupid = @vkGroupId", new { vkgroupid = vkGroup.Id }).SingleOrDefault();
                return lastPostDate;
            }
        }

        public IEnumerable<Post> GetMostPopularPosts(int projectId, PopularBy popularBy, DateRange dateRange, int postsCount)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string qs = dateRange.IsSpecified
                                ? "select itemid::character varying as Item, count(*)::integer as Value from membershare where vkgroupid = @vkgroupid and itemid = any(select vkid::integer from post where vkgroupid = @vkgroupid and posteddate between  @startDate and  @endDate) group by itemid"
                                : "select itemid::character varying as Item, count(*)::integer as Value from membershare where vkgroupid = @vkgroupid and itemid = any(select vkid::integer from post where vkgroupid = @vkgroupid) group by itemid";
                IList<GroupedObject<string>> ps = dataGateway.Connection.Query<GroupedObject<string>>(qs, new { vkgroupid = vkGroup.Id, startDate = dateRange.From, endDate = dateRange.To }).ToList();
                var s = ps.ToDictionary(vc => vc.Item);

                string query = dateRange.IsSpecified
                                   ? @"select * from post p where p.vkgroupid = @vkgroupid and (posteddate >= @startDate and posteddate <= @endDate) order by (likescount + commentscount) desc limit @postscount"
                                   : @"select * from post p where p.vkgroupid = @vkgroupid order by (likescount + commentscount) desc limit @postscount";

                IEnumerable<Post> posts = dataGateway.Connection.Query<Post>(query, new { vkgroupid = vkGroup.Id, postscount = postsCount, startDate = dateRange.From, endDate = dateRange.To });

                foreach (var post in posts)
                {
                    if (s.ContainsKey(post.VkId))
                    {
                        post.SharesCount = s[post.VkId].Value;
                    }
                }

                var topPosts = posts.OrderByDescending(v => v.LikesCount + v.CommentsCount + v.SharesCount).Take(postsCount).ToList();
                return topPosts;
            }
        }

        public IList<MemberInterestsGroupedObject> GetMemberInterest(int projectId, long[] userIds, int? count = 50)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query;

                if (userIds == null || userIds.Length == 0)
                {
                    query = "SELECT lower(title) AS Title, type AS Type, COUNT(*)::integer AS Count FROM memberinterest WHERE vkgroupid = @groupId AND lower(title) <> '' AND length(lower(title)) >= 2 AND length(lower(title)) <= 10 GROUP BY lower(title),type ORDER BY count" + (count.HasValue ? " DESC LIMIT @limit" : string.Empty);
                }
                else
                {
                    query = string.Format("SELECT lower(title) AS Title, type AS Type, COUNT(*)::integer AS Count FROM memberinterest WHERE vkgroupid = @groupId AND lower(title) <> '' AND length(lower(title)) >= 2 AND length(lower(title)) <= 10 AND vkmemberid = ANY ({0}) GROUP BY lower(title),type ORDER BY count DESC LIMIT @limit", QueryArrayBuilder.GetString(userIds));
                }
                return dataGateway.Connection.Query<MemberInterestsGroupedObject>(query, new { groupId = vkGroup.Id, limit = count }).ToList();
            }
        }

        public IList<MemberInterestsGroupedObject> GetMemberInterest(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = "SELECT lower(title) AS Title, type AS Type, COUNT(*)::integer AS Count FROM memberinterest WHERE vkgroupid = @groupId AND lower(title) <> '' AND length(lower(title)) >= 2 AND length(lower(title)) <= 10 GROUP BY lower(title),type ORDER BY count DESC";

                return dataGateway.Connection.Query<MemberInterestsGroupedObject>(query, new { groupId = vkGroupId }).ToList();
            }
        }

        public IList<Photo> GetMostPopularPhotos(int projectId, PopularBy popularBy, DateRange dateRange, int count)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string qs = dateRange.IsSpecified
                                ? "select itemid::character varying as Item, count(*)::integer as Value from membershare where vkgroupid = @vkgroupid and itemid = any(select vkid::integer from photo where vkgroupid = @vkgroupid and posteddate between  @startDate and  @endDate) group by itemid"
                                : "select itemid::character varying as Item, count(*)::integer as Value from membershare where vkgroupid = @vkgroupid and itemid = any(select vkid::integer from photo where vkgroupid = @vkgroupid) group by itemid";
                IList<GroupedObject<string>> ps = dataGateway.Connection.Query<GroupedObject<string>>(qs, new { vkgroupid = vkGroup.Id, startDate = dateRange.From, endDate = dateRange.To }).ToList();
                var s = ps.ToDictionary(vc => vc.Item);

                string query = dateRange.IsSpecified
                                   ? @"select * from photo p where p.vkgroupid = @vkgroupid and (posteddate >= @startDate and posteddate <= @endDate) order by (likescount + commentscount) desc limit @count"
                                   : @"select * from photo p where p.vkgroupid = @vkgroupid order by (likescount + commentscount) desc limit @count";

                IList<Photo> content = dataGateway.Connection.Query<Photo>(query, new { vkgroupid = vkGroup.Id, count = count, startDate = dateRange.From, endDate = dateRange.To }).ToList();

                foreach (var n in content)
                {
                    if (s.ContainsKey(n.VkId))
                    {
                        n.ShareCount = s[n.VkId].Value;
                    }
                }

                var result = content.OrderByDescending(v => v.LikesCount + v.CommentsCount + v.ShareCount).Take(count).ToList();
                return result;
            }
        }

        public IList<Video> GetMostPopularVideos(int projectId, PopularBy popularBy, DateRange dateRange, int count)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string commentsQuery = dateRange.IsSpecified
                                           ? @"select vkvideoid as Item, count(*)::int as Value from videocomment vc where vc.vkgroupid = @vkgroupid and (posteddate >= @startDate and posteddate <= @endDate) group by vkvideoid"
                                           : @"select vkvideoid as Item, count(*)::int as Value from videocomment vc where vc.vkgroupid = @vkgroupid group by vkvideoid";

                IList<GroupedObject<string>> videocomments = dataGateway.Connection.Query<GroupedObject<string>>(commentsQuery, new { vkgroupid = vkGroup.Id, startDate = dateRange.From, endDate = dateRange.To }).ToList();
                var commentsHash = videocomments.ToDictionary(vc => vc.Item);

                string qs = dateRange.IsSpecified
                                ? "select itemid::character varying as Item, count(*)::integer as Value from membershare where vkgroupid = @vkgroupid and itemid = any(select vkid::integer from video where vkgroupid = @vkgroupid and posteddate between  @startDate and  @endDate) group by itemid"
                                : "select itemid::character varying as Item, count(*)::integer as Value from membershare where vkgroupid = @vkgroupid and itemid = any(select vkid::integer from video where vkgroupid = @vkgroupid) group by itemid";
                IList<GroupedObject<string>> ps = dataGateway.Connection.Query<GroupedObject<string>>(qs, new { vkgroupid = vkGroup.Id, startDate = dateRange.From, endDate = dateRange.To }).ToList();
                var s = ps.ToDictionary(vc => vc.Item);

                string query = dateRange.IsSpecified
                                   ? @"select * from video v where v.vkgroupid = @vkgroupid and (posteddate >= @startDate and posteddate <= @endDate) order by likescount"
                                   : @"select * from video v where v.vkgroupid = @vkgroupid order by likescount desc limit @count";

                IList<Video> videos = dataGateway.Connection.Query<Video>(query, new { vkgroupid = vkGroup.Id, count = count, startDate = dateRange.From, endDate = dateRange.To }).ToList();

                foreach (var video in videos)
                {
                    if (commentsHash.ContainsKey(video.VkId))
                    {
                        video.CommentsCount = commentsHash[video.VkId].Value;
                    }
                    if (s.ContainsKey(video.VkId))
                    {
                        video.ShareCount = s[video.VkId].Value;
                    }
                }

                var topVideos = videos.OrderByDescending(v => v.LikesCount + v.CommentsCount + v.ShareCount).Take(count).ToList();
                return topVideos;
            }
        }

        public IEnumerable<PointInTime> GetPhotos(int projectId, DateRange range, Periodicity periodicity)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IQueryable<VkEntity> query = dataGateway.GetEntities<Photo>().Where(p => p.VkGroupId == vkGroup.Id && p.PostedDate >= range.From && p.PostedDate <= range.To);
                return this.entitiesForChartProvider.GetEntitiesForChart(query, range, periodicity, entities => entities.Count());
            }
        }

        public IEnumerable<PointInTime> GetPosts(int projectId, DateRange range, Periodicity periodicity)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IQueryable<VkEntity> query = dataGateway.GetEntities<Post>().Where(p => p.VkGroupId == vkGroup.Id && p.PostedDate >= range.From && p.PostedDate <= range.To);
                return this.entitiesForChartProvider.GetEntitiesForChart(query, range, periodicity, entities => entities.Count());
            }
        }

        public IEnumerable<IEnumerable<PointInTime>> GetUsersCount(int projectId, DateRange range, Periodicity periodicity)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IQueryable<MembersMetaInfo> query = dataGateway.GetEntities<MembersMetaInfo>().Where(p => p.VkGroupId == vkGroup.Id && p.PostedDate >= range.From && p.PostedDate <= range.To);

                /* const string MembersDeltaQuery = @"SELECT * FROM membersdelta WHERE vkgroupid=@vkgroupid AND posteddate BETWEEN @from AND @to";
                var deltas = dataGateway.Connection.Query<MembersDelta>(MembersDeltaQuery, new { vkgroupid = vkGroup.Id, from = range.From, to = range.To }).AsQueryable();
                var comeMembers = this.entitiesForChartProvider.GetEntitiesForChart(deltas, range, periodicity, x => x.Sum(i => i.InCount));
                var leftMembers = this.entitiesForChartProvider.GetEntitiesForChart(deltas, range, periodicity, x => x.Sum(i => i.OutCount)); 
                return new List<IEnumerable<PointInTime>> { users, comeMembers, leftMembers }; */
                
                var users = this.GetCounterAverages(query.Where(x => x != null).ToList(), range, periodicity);
                IFillEmptingsStrategy strategy = FillEmptingsStrategyFactory.Create<MembersMetaInfo>();
                users = this.FillEmptings(users, range, periodicity, strategy);
                return new List<IEnumerable<PointInTime>> { users };
            }
        }

        public IEnumerable<IEnumerable<PointInTime>> GetDashboardDataChart(int projectId, DateRange range, Periodicity periodicity)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            List<Post> postList = new List<Post>();

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                postList =
                    dataGateway.GetEntities<Post>()
                               .Where(
                                   p =>
                                   p.VkGroupId == vkGroup.Id && p.PostedDate >= range.From && p.PostedDate <= range.To)
                               .ToList();
            }

            List<Post> memberPosts = new List<Post>();
            List<Post> adminPosts = new List<Post>();
            for (var i = 0; i < postList.Count; i++)
            {
                if (this.IsMemberAdmin(postList[i].CreatorId, vkGroup.Id))
                {
                    adminPosts.Add(postList[i]);
                }
                else
                {
                    memberPosts.Add(postList[i]);
                }
            }
            List<PointInTime> memberPostData = this.entitiesForChartProvider.GetEntitiesForChart(memberPosts.AsQueryable(), range, periodicity, x => x.Count()).ToList();
            List<PointInTime> adminPostData = this.entitiesForChartProvider.GetEntitiesForChart(adminPosts.AsQueryable(), range, periodicity, x => x.Count()).ToList();
            List<PointInTime> total = this.entitiesForChartProvider.GetEntitiesForChart(postList.AsQueryable(), range, periodicity, x => x.Count()).ToList();
            return new List<IEnumerable<PointInTime>>() { adminPostData.AsEnumerable(), memberPostData.AsEnumerable(), total.AsEnumerable() };
        }

        public CategorialValue GetEducationLevelInformation(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                CategorialValue educationInfo = new CategorialValue();
                string query = string.Format("SELECT education AS Item, COUNT(*)::integer AS Value FROM member WHERE vkgroupid = {0} GROUP BY education", vkGroup.Id);
                IDictionary<EducationLevel, GroupedObject<EducationLevel>> groupedObjects = dataGateway.Connection.Query<GroupedObject<EducationLevel>>(query).ToDictionary(x => x.Item);
                educationInfo.CategoryA = groupedObjects.ContainsKey(EducationLevel.Unknown) ? groupedObjects[EducationLevel.Unknown].Value : 0;
                educationInfo.CategoryB = groupedObjects.ContainsKey(EducationLevel.Middle) ? groupedObjects[EducationLevel.Middle].Value : 0;
                educationInfo.CategoryC = groupedObjects.ContainsKey(EducationLevel.UncompletedHigher) ? groupedObjects[EducationLevel.UncompletedHigher].Value : 0;
                educationInfo.CategoryD = groupedObjects.ContainsKey(EducationLevel.Higher) ? groupedObjects[EducationLevel.Higher].Value : 0;
                educationInfo.CategoryE = groupedObjects.ContainsKey(EducationLevel.PhD) ? groupedObjects[EducationLevel.PhD].Value : 0;

                return educationInfo;
            }
        }

        public long GetAverageResponseTimeInTick(int projectId, DateRange range)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            List<PostComment> postCommentList;
            List<Post> postList;
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query =
                    @"select * from (select distinct on (vkpostid) * from postcomment where vkgroupid = @groupId and creatorid = any(select userid from administrator where vkgroupid = @groupId)) as sub order by posteddate";
                postCommentList = dataGateway.Connection.Query<Ix.Palantir.DomainModel.PostComment>(query, new { groupId = vkGroup.Id, from = range.From, to = range.To }).ToList();
                query = @"select * from post where vkgroupid = @groupId and vkid = any(select vkpostid from (select distinct on (vkpostid) * from postcomment where vkgroupid = @groupId and creatorid = any(select userid from administrator where vkgroupid = @groupId)) as sub order by posteddate) and creatorid <> all(select userid from administrator where vkgroupid = @groupId) and posteddate between @from and @to order by posteddate";
                postList =
                    dataGateway.Connection.Query<Ix.Palantir.DomainModel.Post>(query, new { groupId = vkGroup.Id, from = range.From, to = range.To }).ToList();
            }

            List<long> tempResult = new List<long>();
            for (int i = 0; i < postList.Count; i++)
            {
                var comment = postCommentList.FirstOrDefault(x => x.VkPostId == postList[i].VkId);
                if (comment != null)
                {
                    tempResult.Add(comment.PostedDate.Ticks - postList[i].PostedDate.Ticks);
                }
            }

                if (tempResult.Count != 0)
                {
                    return tempResult.Sum() / tempResult.Count;
                }
            return -1;
        }

        public IEnumerable<PointInTime> GetVideos(int projectId, DateRange range, Periodicity periodicity)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IQueryable<VkEntity> query = dataGateway.GetEntities<Video>().Where(p => p.VkGroupId == vkGroup.Id && p.PostedDate >= range.From && p.PostedDate <= range.To);
                return this.entitiesForChartProvider.GetEntitiesForChart(query, range, periodicity, entities => entities.Count());
            }
        }

        public IEnumerable<GroupedObject<long>> GetVideoCommentCountByGroupAndDate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT creatorid AS Item, COALESCE(SUM(count)::integer, 0) AS Value FROM (SELECT creatorid AS creatorid, 1 AS count FROM videocomment WHERE vkgroupid = @groupid AND posteddate BETWEEN @from AND @to) AS SUB GROUP BY creatorid";
                return dataGateway.Connection.Query<GroupedObject<long>>(query, new { groupid = vkGroup.Id, from = dateRange.From, to = dateRange.To });
            }
        }

        public IEnumerable<ActiveUser> GetActiveUsers(int projectId, DateRange dateRange, int count = 40)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string dateFilter = dateRange != null
                    ? string.Format(" AND posteddate BETWEEN '{0}' AND '{1}'", QueryDateBuilder.GetDateString(dateRange.From), QueryDateBuilder.GetDateString(dateRange.To))
                    : string.Empty;
                string limitFilter = count > 0
                    ? string.Format(" LIMIT {0}", count)
                    : string.Empty;

                string query = string.Format(@"DROP TABLE IF EXISTS result;CREATE TEMPORARY TABLE result(creatorid bigint,postcount int,postcommentcount int,topiccommentcount int,videocommentcount int, sharecount int, likecount int);INSERT INTO result SELECT creatorid AS creatorid, SUM(count)::integer AS postcount, 0::integer AS postcommentcount, 0::integer AS topiccommentcount, 0::integer AS videocommentcount, 0::integer AS sharecount, 0::integer AS likecount FROM (SELECT creatorid AS creatorid, 1 AS count FROM post WHERE vkgroupid = {0} {1}) AS SUB GROUP BY creatorid;INSERT INTO result SELECT creatorid AS creatorid, 0::integer AS postcount, SUM(count)::integer AS postcommentcount, 0::integer AS topiccommentcount, 0::integer AS videocommentcount, 0::integer AS sharecount, 0::integer AS likecount FROM (SELECT creatorid AS creatorid, 1 AS count FROM postcomment WHERE vkgroupid = {0} {1}) AS SUB GROUP BY creatorid;INSERT INTO result SELECT creatorid AS creatorid, 0::integer AS postcount, 0::integer AS postcommentcount, SUM(count)::integer AS topiccommentcount, 0::integer AS videocommentcount, 0::integer AS sharecount, 0::integer AS likecount FROM (SELECT creatorid AS creatorid, 1 AS count FROM topiccomment WHERE vkgroupid = {0} {1}) AS SUB GROUP BY creatorid;INSERT INTO result SELECT creatorid AS creatorid, 0::integer AS postcount, 0::integer AS postcommentcount, 0::integer AS topiccommentcount, SUM(count)::integer AS videocommentcount, 0::integer AS sharecount, 0::integer AS likecount FROM (SELECT creatorid AS creatorid, 1 AS count FROM videocomment WHERE vkgroupid = {0} {1}) AS SUB GROUP BY creatorid;INSERT INTO result SELECT creatorid AS creatorid, 0::integer AS postcount, 0::integer AS postcommentcount, 0::integer AS topiccommentcount, 0::integer AS videocommentcount, SUM(count)::integer AS sharecount, 0::integer AS likecount FROM (SELECT vkmemberid AS creatorid, 1 AS count FROM membershare WHERE vkgroupid = {0} AND itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = {0} {1} UNION ALL SELECT vkid::integer FROM photo WHERE vkgroupid = {0} {1} UNION ALL SELECT vkid::integer FROM video WHERE vkgroupid = {0} {1})) AS SUB GROUP BY creatorid;INSERT INTO result SELECT creatorid AS creatorid, 0::integer AS postcount, 0::integer AS postcommentcount, 0::integer AS topiccommentcount, 0::integer AS videocommentcount, 0::integer AS sharecount, SUM(count)::integer AS likecount FROM (SELECT vkmemberid AS creatorid, 1 AS count FROM memberlike WHERE vkgroupid = {0} AND itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = {0} {1} UNION ALL SELECT vkid::integer FROM photo WHERE vkgroupid = {0} {1} UNION ALL SELECT vkid::integer FROM video WHERE vkgroupid = {0} {1})) AS SUB GROUP BY creatorid;SELECT SUB.creatorid AS Id, SUB.postcount, SUB.commentcount, SUB.likecount, SUB.sharecount, m.name AS UserName, m.gender, m.education, m.cityid, m.countryid, m.birthday, m.birthmonth, m.birthyear FROM (SELECT creatorid, (postcommentcount + topiccommentcount + videocommentcount + sharecount + likecount) AS CommentCount, PostCount, LikeCount, sharecount FROM (SELECT creatorid AS creatorid, SUM(postcount)::integer AS postcount, SUM(postcommentcount)::integer AS postcommentcount, SUM(topiccommentcount)::integer AS topiccommentcount, SUM(videocommentcount)::integer AS videocommentcount, SUM(sharecount)::integer AS sharecount, SUM(likecount)::integer AS likecount FROM result GROUP BY creatorid) AS SUB ORDER BY (postcount + postcommentcount + topiccommentcount + videocommentcount + sharecount + likecount) DESC {2}) AS SUB INNER JOIN member m ON (m.vkgroupid = {0} and SUB.creatorid = m.vkmemberid)", vkGroup.Id, dateFilter, limitFilter);

                return dataGateway.Connection.Query<ActiveUser, BirthDate, ActiveUser>(
                    query, 
                    (member, birthdate) => { member.BirthDate = birthdate; return member; },
                    null,
                    splitOn: "birthday").ToList();
            }
        }

        public int GetPhotosCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select count(*)::int as photocount from photo where vkgroupid = @vkgroupid"
                               + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                int count = dataGateway.Connection.Query<int>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                return count;
            }
        }

        public int GetVideosCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select count(*)::int as videocount from video where vkgroupid = @vkgroupid"
                               + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                int count = dataGateway.Connection.Query<int>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                return count;
            }
        }

        public int GetPostsCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select count(*)::int as postcount from post where vkgroupid = @vkgroupid"
                               + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                int count = dataGateway.Connection.Query<int>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                return count;
            }
        }

        public int GetTopicsCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select count(*)::int as topiccount from topic where vkgroupid = @vkgroupid"
                               + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                int count = dataGateway.Connection.Query<int>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                return count;
            }
        }

        public int GetTopicCommentsCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select count(*)::int as topiccommentscount from topiccomment where vkgroupid = @vkgroupid"
                               + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                int count = dataGateway.Connection.Query<int>(query, new { vkgroupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                return count;
            }
        }

        public IEnumerable<float> GetPostAverageCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int likesCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM postcomment WHERE vkgroupid = @groupId AND  vkpostid = ANY(SELECT vkid FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int commentsCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int shareCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to";
                int postCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                postCount = postCount == 0 ? 1 : postCount;

                return new float[3] { (float)likesCount / (float)postCount, (float)commentsCount / (float)postCount, (float)shareCount / (float)postCount };
            }
        }

        public IEnumerable<float> GetPhotoAverageCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int likesCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COALESCE(SUM(commentscount)::integer, 0) FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to";
                int commentsCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int shareCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to";
                int photoCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                photoCount = photoCount == 0 ? 1 : photoCount;

                return new float[3] { (float)likesCount / (float)photoCount, (float)commentsCount / (float)photoCount, (float)shareCount / (float)photoCount };
            }
        }

        public IEnumerable<float> GetVideoAverageCount(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int likesCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM videocomment WHERE vkgroupid = @groupId AND  vkvideoid = ANY(SELECT vkid FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int commentsCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int shareCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to";
                int photoCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroup.Id, from = dateRange.From, to = dateRange.To }).SingleOrDefault();
                photoCount = photoCount == 0 ? 1 : photoCount;

                return new float[3] { (float)likesCount / (float)photoCount, (float)commentsCount / (float)photoCount, (float)shareCount / (float)photoCount };
            }
        }

        public IEnumerable<GroupedObject<long>> GetPostsCountByGroupAndDate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT creatorid AS Item, SUM(count)::integer AS Value FROM (SELECT creatorid AS creatorid, 1 AS count FROM post WHERE vkgroupid = @groupid AND posteddate BETWEEN @from AND @to) AS SUB GROUP BY creatorid";
                IList<GroupedObject<long>> interests = dataGateway.Connection.Query<GroupedObject<long>>(query, new { groupid = vkGroup.Id, from = dateRange.From, to = dateRange.To }).ToList();
                return interests;
            }
        }

        public IEnumerable<GroupedObject<long>> GetPostsCommentCountByGroupAndDate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT creatorid AS Item, SUM(count)::integer AS Value FROM (SELECT creatorid AS creatorid, 1 AS count FROM postcomment WHERE vkgroupid = @groupid AND posteddate BETWEEN @from AND @to) AS SUB GROUP BY creatorid";
                return dataGateway.Connection.Query<GroupedObject<long>>(query, new { groupid = vkGroup.Id, from = dateRange.From, to = dateRange.To });
            }
        }

        public IEnumerable<GroupedObject<long>> GetTopicCommentCountByGroupAndDate(int projectId, DateRange dateRange)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT creatorid AS Item, SUM(count)::integer AS Value FROM (SELECT creatorid AS creatorid, 1 AS count FROM topiccomment WHERE vkgroupid = @groupid AND posteddate BETWEEN @from AND @to) AS SUB GROUP BY creatorid";
                return dataGateway.Connection.Query<GroupedObject<long>>(query, new { groupid = vkGroup.Id, from = dateRange.From, to = dateRange.To });
            }
        }

        public IEnumerable<LocationInfoGroupedObject> GetMostPopularCitiesActiveUsers(int projectId, long[] usersId, int cityCount = 30)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var query = string.Format("select c.title as City, cnt.title as Country, count(*)::integer as Count from member m join city c on (m.cityid = c.vkid::integer) join country cnt on (m.countryid = cnt.vkid::integer) where m.vkgroupid = @vkGroupId AND m.vkmemberid=ANY({0}) group by c.title, cnt.title order by count(*) desc limit @cityCount", QueryArrayBuilder.GetString(usersId));
                IList<LocationInfoGroupedObject> cities = dataGateway.Connection.Query<LocationInfoGroupedObject>(query, new { vkgroupid = vkGroup.Id, cityCount }).ToList();
                return cities.Where(c => !string.IsNullOrWhiteSpace(c.City));
            }
        }

        public int GetMembersWithoutCityCount(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                int count = dataGateway.GetEntities<Member>().Count(m => m.VkGroupId == vkGroup.Id && m.CityId == 0);
                return count;
            }
        }

        public IList<City> GetCities(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var cities = dataGateway.Connection.Query<City>("select distinct c.* as City from member m join city c on (m.cityid = c.vkid::integer) where m.vkgroupid = @vkGroupId order by c.title", new { vkGroupId = vkGroup.Id }).ToList();
                return cities.ToList();
            }
        }

        public DateTime? GetFirstPostDate(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                DateTime? firstPostDate = dataGateway.Connection.Query<DateTime?>("select min(posteddate) from post where vkgroupid = @vkGroupId", new { vkgroupid = vkGroup.Id }).SingleOrDefault();
                return firstPostDate;
            }
        }

        public DateTime? GetLastPostDate(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                DateTime? lastPostDate = dataGateway.Connection.Query<DateTime?>("select max(posteddate) from post where vkgroupid = @vkGroupId", new { vkgroupid = vkGroup.Id }).SingleOrDefault();
                return lastPostDate;
            }
        }

        public DateTime? GetFirstMemberDate(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                DateTime? firstPostDate = dataGateway.Connection.Query<DateTime?>("select min(posteddate) from membersmetainfo where vkgroupid = @vkGroupId", new { vkgroupid = vkGroup.Id }).SingleOrDefault();
                return firstPostDate;
            }
        }

        public CategorialValue GetGenderInformation(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                CategorialValue genderInfo = new CategorialValue();
                IDictionary<Gender, GroupedObject<Gender>> groupedObjects = dataGateway.Connection.Query<GroupedObject<Gender>>("select gender as Item, COUNT(*)::integer as Value from member where vkgroupid = @vkGroupId group by gender", new { vkgroupid = vkGroup.Id }).ToDictionary(x => x.Item);

                genderInfo.CategoryA = groupedObjects.ContainsKey(Gender.Male) ? groupedObjects[Gender.Male].Value : 0;
                genderInfo.CategoryB = groupedObjects.ContainsKey(Gender.Female) ? groupedObjects[Gender.Female].Value : 0;
                genderInfo.CategoryC = groupedObjects.ContainsKey(Gender.Unknown) ? groupedObjects[Gender.Unknown].Value : 0;

                return genderInfo;
            }
        }

        public CategorialValue GetAgeInformation(int projectId)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<BirthDate> birthdays = dataGateway.Connection.Query<BirthDate>("select birthday, birthmonth, birthyear from member where vkgroupid = @vkGroupId", new { vkgroupid = vkGroup.Id });
                var ageInfo = this.birthdayGrouper.GroupByBirthday(birthdays);

                return ageInfo;
            }
        }

        public IEnumerable<LocationInfoGroupedObject> GetMostPopularCities(int projectId, int cityCount)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<LocationInfoGroupedObject> cities = dataGateway.Connection.Query<LocationInfoGroupedObject>("select c.title as City, cnt.title as Country, count(*)::integer as Count from member m join city c on (m.cityid = c.vkid::integer) join country cnt on (m.countryid = cnt.vkid::integer) where m.vkgroupid = @vkGroupId group by c.title, cnt.title order by count(*) desc limit @cityCount", new { vkgroupid = vkGroup.Id, cityCount }).ToList();
                return cities.Where(c => !string.IsNullOrWhiteSpace(c.City));
            }
        }

        public IEnumerable<GroupedObject<string>> GetGroupInterests(int projectId, int interestsCount)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<GroupedObject<string>> interests = dataGateway.Connection.Query<GroupedObject<string>>("select lower(title) AS Item, count(*)::integer AS Value from memberinterest where vkgroupid = @vkgroupid group by lower(title) order by count(*) desc limit @interestsCount", new { vkgroupid = vkGroup.Id, interestsCount }).ToList();
                return interests;
            }
        }

        public IEnumerable<NamedEntity> GetMembersByInterest(int projectId, string interestTitle)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<NamedEntity> members = dataGateway.Connection.Query<NamedEntity>("select distinct m.name AS Title, m.vkmemberid::integer AS Id from member m join memberinterest mi on (m.vkgroupid = mi.vkgroupid AND m.vkmemberid = mi.vkmemberid) where mi.vkgroupid = @vkgroupid and lower(mi.title) = lower(@title)", new { vkgroupid = vkGroup.Id, title = interestTitle }).ToList();
                return members;
            }
        }

        public int[] GetPostReactionCount(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int likesCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM postcomment WHERE vkgroupid = @groupId AND  vkpostid = ANY(SELECT vkid FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int commentsCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM post WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int shareCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                return new int[3] { likesCount, commentsCount, shareCount };
            }
        }

        public int[] GetPhotoReactionCount(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int likesCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COALESCE(SUM(commentscount)::integer, 0) FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to";
                int commentsCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM photo WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int shareCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                return new int[3] { likesCount, commentsCount, shareCount };
            }
        }

        public int[] GetVideoReactionCount(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int likesCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM videocomment WHERE vkgroupid = @groupId AND  vkvideoid = ANY(SELECT vkid FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int commentsCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                query = @"SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = @groupId AND  itemid = ANY(SELECT vkid::integer FROM video WHERE vkgroupid = @groupId AND posteddate BETWEEN @from AND @to)";
                int shareCount = dataGateway.Connection.Query<int>(query, new { groupId = vkGroupId, from = dateRange.From, to = dateRange.To }).SingleOrDefault();

                return new int[3] { likesCount, commentsCount, shareCount };
            }
        }

        private IEnumerable<PointInTime> GetCounterAverages<T>(IList<T> query, DateRange range, Periodicity periodicity) where T : VkEntity, ICounter
        {
            IList<PointOfAverage> averages = new List<PointOfAverage>();

            switch (periodicity)
            {
                case Periodicity.ByHour:
                    for (var i = range.From; i < range.To; i = i.AddHours(1))
                    {
                        averages.Add(new PointOfAverage()
                            {
                                Date = i,
                                Value =
                                    query.Where(x => (x.PostedDate >= i) && (x.PostedDate < i.AddHours(1)))
                                         .Sum(x => x.Count),
                                Count = query.Count(x => (x.PostedDate >= i) && (x.PostedDate < i.AddHours(1)))
                            });
                    }
                    break;

                case Periodicity.ByDayWithHour:
                    for (var i = range.From; i < range.To; i = i.AddDays(1))
                    {
                        averages.Add(new PointOfAverage()
                            {
                                Date = i,
                                Value =
                                    query.Where(x => (x.PostedDate >= i) && (x.PostedDate < i.AddDays(1)))
                                         .Sum(x => x.Count),
                                Count = query.Count(x => (x.PostedDate >= i) && (x.PostedDate < i.AddDays(1)))
                            });
                    }
                    break;

                case Periodicity.ByDay:
                    for (var i = range.From; i < range.To; i = i.AddDays(1))
                    {
                        averages.Add(new PointOfAverage()
                            {
                                Date = i,
                                Value =
                                    query.Where(x => (x.PostedDate >= i) && (x.PostedDate < i.AddDays(1)))
                                         .Sum(x => x.Count),
                                Count = query.Count(x => (x.PostedDate >= i) && (x.PostedDate < i.AddDays(1)))
                            });
                    }
                    break;

                case Periodicity.ByWeek:
                    for (var i = range.From; i < range.To; i = i.AddDays(7))
                    {
                        averages.Add(new PointOfAverage()
                            {
                                Date = i,
                                Value =
                                    query.Where(x => (x.PostedDate >= i) && (x.PostedDate < i.AddDays(7)))
                                         .Sum(x => x.Count),
                                Count = query.Count(x => (x.PostedDate >= i) && (x.PostedDate < i.AddDays(7)))
                            });
                    }
                    break;

                case Periodicity.ByMonth:
                    for (var i = range.From; i < range.To; i = i.AddMonths(1))
                    {
                        averages.Add(new PointOfAverage()
                            {
                                Date = i,
                                Value = query.Where(x => (x.PostedDate >= i) && (x.PostedDate < i.AddMonths(1))).Sum(x => (long)x.Count),
                                Count = query.Count(x => (x.PostedDate >= i) && (x.PostedDate < i.AddMonths(1)))
                            });
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("periodicity");
            }
            if (averages.Count == 0)
            {
                return null;
            }
            IList<PointInTime> result = new List<PointInTime>(averages.Select(x => x.GetPointInTime()));
            return result;
        }

        private bool IsMemberAdmin(long memberId, int groupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var data = dataGateway.Connection.Query<Administrator>(string.Format("SELECT * FROM administrator WHERE vkgroupid = {0} AND userid = {1}", groupId, memberId));
                return data.Count() != 0;
            }
        }

        private IList<PointInTime> FillEmptings(IEnumerable<PointInTime> result, DateRange range, Periodicity periodicity, IFillEmptingsStrategy strategy)
        {
            IDictionary<DateTime, PointInTime> actualData = result.ToDictionary(x => x.Date);
            IList<PointInTime> actualDataList = actualData.Values.ToList();
            IList<PointInTime> resultWithoutEmptings = new List<PointInTime>();
            Func<DateTime, DateTime> getNextDate = this.GetNextDateGetter(periodicity);

            DateTime currentDate = range.From;
            int index = 0;

            while (currentDate < range.To)
            {
                bool scoreExists = actualData.ContainsKey(currentDate);
                PointInTime score = scoreExists ? actualData[currentDate] : null;
                resultWithoutEmptings.Add(new PointInTime { Date = currentDate, Value = score != null && score.Value > 0 ? score.Value : strategy.GetValue(actualDataList, index, scoreExists) });
                currentDate = getNextDate(currentDate);

                if (scoreExists)
                {
                    index++;
                }
            }

            return resultWithoutEmptings;
        }
        private Func<DateTime, DateTime> GetNextDateGetter(Periodicity periodicity)
        {
            Func<DateTime, DateTime> getNextDate;
            switch (periodicity)
            {
                case Periodicity.ByHour:
                case Periodicity.ByDayWithHour:
                    getNextDate = x => x.AddHours(1);
                    break;

                case Periodicity.ByDay:
                    getNextDate = x => x.AddDays(1);
                    break;

                case Periodicity.ByWeek:
                    getNextDate = x => x.AddDays(7);
                    break;

                case Periodicity.ByMonth:
                    getNextDate = x => x.AddMonths(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("periodicity");
            }
            return getNextDate;
        }
    }
}