namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public class RawDataProvider : IRawDataProvider
    {
        private const string CONST_DateRangeSuffix = " and posteddate >= @from and posteddate <= @to";

        private readonly IDataGatewayProvider dataGatewayProvider;

        public RawDataProvider(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public IList<Post> GetPosts(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<Post> posts = !dateRange.IsSpecified 
                                  ? dataGateway.Connection.Query<Post>("select p.vkid as VkId, p.posteddate as PostedDate, p.text as Text, p.creatorid as CreatorId, p.likescount as LikesCount, p.commentscount as CommentsCount from post p where p.vkgroupid = @vkgroupid order by p.posteddate desc", new { vkgroupid = vkGroupId }).ToList() 
                                  : dataGateway.Connection.Query<Post>("select p.vkid as VkId, p.posteddate as PostedDate, p.text as Text, p.creatorid as CreatorId, p.likescount as LikesCount, p.commentscount as CommentsCount from post p where p.vkgroupid = @vkGroupId and p.posteddate >= @from and p.posteddate <= @to order by p.posteddate desc", new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();

                return posts;
            }
        }
        public IList<Member> GetMembers(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                const string QueryMember = @"select id, vkgroupid, name, gender, maritalstatus, cityid, countryid, education, vkmemberid, status, birthday, birthmonth, birthyear from member where vkgroupid = @vkgroupid;";
                const string QueryInterests = @"select * from memberinterest where vkgroupid = @vkgroupid";

                var members = dataGateway.Connection.Query<Member, BirthDate, Member>(
                    QueryMember,
                    (member, birthdate) => { member.BirthDate = birthdate; return member; }, 
                    new { vkgroupid = vkGroupId },
                    splitOn: "birthday").ToList();
                var interests = dataGateway.Connection.Query<MemberInterest>(QueryInterests, new { vkgroupid = vkGroupId }).ToList();

                var membersDictionary = members.ToDictionary(m => m.VkMemberId);

                foreach (var interest in interests)
                {
                    if (membersDictionary.ContainsKey(interest.VkMemberId))
                    {
                        membersDictionary[interest.VkMemberId].Interests.Add(interest);
                    }
                }

                return members;
            }
        }
        public IList<MemberLike> GetMemberLikes(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<MemberLike> likes = dataGateway.Connection.Query<MemberLike>("select * from memberlike v where v.vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId }).ToList();
                return likes;
            }
        }
        public IList<MemberShare> GetMemberShares(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<MemberShare> shares = dataGateway.Connection.Query<MemberShare>("select * from membershare v where v.vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId }).ToList();
                return shares;
            }
        }
        public IList<MemberShare> GetMembersPosts(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<MemberShare> shares = dataGateway.Connection.Query<MemberShare>("select * from post v where v.vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId }).ToList();
                return shares;
            }
        }
        public IList<MemberStatExport> GetMemberStatInfo(int vkGroupId, string entityName, string groupKey)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<MemberStatExport> result = dataGateway.Connection.Query<MemberStatExport>(string.Format("select count(*) as EntityCount, {0} as CreatorId from {1} where vkgroupid = @vkgroupid group by {0}", groupKey, entityName), new { vkgroupid = vkGroupId }).ToList();
                return result;
            }
        }
        public IList<Video> GetVideos(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<Video> videos = !dateRange.IsSpecified
                                  ? dataGateway.Connection.Query<Video>("select v.vkid as VkId, v.posteddate as PostedDate, v.title as Title, v.description as Description, v.duration as Duration from video v where v.vkgroupid = @vkgroupid order by v.posteddate desc", new { vkgroupid = vkGroupId }).ToList()
                                  : dataGateway.Connection.Query<Video>("select v.vkid as VkId, v.posteddate as PostedDate, v.title as Title, v.description as Description, v.duration as Duration from video v where v.vkgroupid = @vkgroupid and v.posteddate >= @from and v.posteddate <= @to order by v.posteddate desc", new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();

                return videos;
            }
        }
        public IList<Photo> GetPhotos(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<Photo> photos = !dateRange.IsSpecified
                                  ? dataGateway.Connection.Query<Photo>("select p.vkid as VkId, p.albumId as AlbumId, p.posteddate as PostedDate from photo p where p.vkgroupid = @vkgroupid order by p.posteddate desc", new { vkgroupid = vkGroupId }).ToList()
                                  : dataGateway.Connection.Query<Photo>("select p.vkid as VkId, p.albumId as AlbumId, p.posteddate as PostedDate from photo p where p.vkgroupid = @vkgroupid and p.posteddate >= @from and p.posteddate <= @to order by p.posteddate desc", new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();

                return photos;
            }
        }
        public IDictionary<int, City> GetCities()
        {
            IDictionary<int, City> cities = new Dictionary<int, City>();
            IList<City> citiesList;

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                citiesList = dataGateway.Connection.Query<City>("select * from city").ToList();
            }

            foreach (var city in citiesList)
            {
                var vkId = int.Parse(city.VkId);

                if (!cities.ContainsKey(vkId))
                {
                    cities.Add(vkId, city);
                }
            }

            return cities;
        }
        public IDictionary<int, Country> GetCountries()
        {
            IDictionary<int, Country> countries = new Dictionary<int, Country>();
            IList<Country> countriesList;

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                countriesList = dataGateway.Connection.Query<Country>("select * from country").ToList();
            }

            foreach (var country in countriesList)
            {
                var vkId = int.Parse(country.VkId);

                if (!countries.ContainsKey(vkId))
                {
                    countries.Add(vkId, country);
                }
            }

            return countries;
        }

        public IList<long> GetPostCreatorIds(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select distinct creatorid from post where vkgroupid = @vkgroupid"
                + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                var creatorIds = dataGateway.Connection.Query<long>(query, new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();
                return creatorIds;
            }
        }

        public IList<long> GetPostCommentCreatorIds(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select distinct creatorid from postcomment where vkgroupid = @vkgroupid"
                + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                var creatorIds = dataGateway.Connection.Query<long>(query, new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();
                return creatorIds;
            }
        }

        public IList<long> GetTopicCreatorIds(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select distinct createdbyvkid from topic where vkgroupid = @vkgroupid"
                + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                var creatorIds = dataGateway.Connection.Query<long>(query, new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();
                return creatorIds;
            }
        }

        public IList<long> GetTopicCommentCreatorIds(int vkGroupId, DateRange dateRange)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select distinct creatorid from topiccomment where vkgroupid = @vkgroupid"
                + (dateRange.IsSpecified ? CONST_DateRangeSuffix : string.Empty);

                var creatorIds = dataGateway.Connection.Query<long>(query, new { vkgroupid = vkGroupId, from = dateRange.From, to = dateRange.To }).ToList();
                return creatorIds;
            }
        }

        public IList<MemberWithStatus> GetUserIdsWithStatus(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select vkmemberid as MemberId, status as Status from member where vkgroupid = @vkgroupid";

                var creatorIds = dataGateway.Connection.Query<MemberWithStatus>(query, new { vkgroupid = vkGroupId }).ToList();
                return creatorIds;
            }
        }

        public int GetMembersCount(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = @"select count(*)::int as memberscount from member where vkgroupid = @vkgroupid";

                var usersCount = dataGateway.Connection.Query<int>(query, new { vkgroupid = vkGroupId }).SingleOrDefault();
                return usersCount;
            }
        }
    }
}