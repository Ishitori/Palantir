namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;
    using Ix.Palantir.DataAccess.StatisticsProviders.Helpers;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Utilities;

    public class MemberAdvancedSearcher : IMemberAdvancedSearcher
    {
        private readonly IProjectRepository projectRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly BirthdayGrouper birthdayGrouper;

        public MemberAdvancedSearcher(IProjectRepository projectRepository, IDataGatewayProvider dataGatewayProvider)
        {
            this.projectRepository = projectRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.birthdayGrouper = new BirthdayGrouper();
        }

        public IList<MemberMainInfo> GetAudienceByFilter(int projectId, AudienceFilter filter)
        {
            var vkGroup = this.projectRepository.GetVkGroup(projectId);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.Connection.Query<MemberMainInfo, BirthDate, MemberMainInfo>(
                    QueryMemberFilterBuilder.GetString(vkGroup.Id, filter),
                    (member, birthdate) => { member.BirthDate = birthdate; return member; },
                    new { vkgroupid = vkGroup.Id },
                    splitOn: "birthday").ToList(); 
            }
        }

        public CategorialValue GetAgeInformationActiveUsers(int vkGroupId, AudienceFilteringResult result, bool withoutMonthAndDay = false)
        {
            IEnumerable<BirthDate> birthdays = result.Members.Select(m => m.BirthDate);
            var ageInfo = this.birthdayGrouper.GroupByBirthday(birthdays, withoutMonthAndDay);

            return ageInfo;
        }
        public CategorialValue GetGenderInformationActiveUsers(int vkGroupId, AudienceFilteringResult result)
        {
            var genderInfo = new CategorialValue();

            foreach (var member in result.Members)
            {
                switch (member.Gender)
                {
                    case Gender.Male:
                        genderInfo.CategoryA++;
                        break;

                    case Gender.Female:
                        genderInfo.CategoryB++;
                        break;

                    case Gender.Unknown:
                        genderInfo.CategoryC++;
                        break;
                }
            }

            return genderInfo;
        }
        public CategorialValue GetEducationLevelInformationActiveUsers(int vkGroupId, AudienceFilteringResult result)
        {
            var educationInfo = new CategorialValue();

            foreach (var member in result.Members)
            {
                switch (member.Education)
                {
                    case EducationLevel.Unknown:
                        educationInfo.CategoryA++;
                        break;

                    case EducationLevel.Middle:
                        educationInfo.CategoryB++;
                        break;
                    
                    case EducationLevel.UncompletedHigher:
                        educationInfo.CategoryC++;
                        break;
                    
                    case EducationLevel.Higher:
                        educationInfo.CategoryD++;
                        break;
                    
                    case EducationLevel.PhD:
                        educationInfo.CategoryE++;
                        break;
                }
            }

            return educationInfo;
        }

        public IEnumerable<GroupedObject<Country>> GetCountryInformationActiveUsers(int vkGroupId, AudienceFilteringResult result)
        {
            IDictionary<int, int> memberPerCountry = new Dictionary<int, int>();

            foreach (var member in result.Members)
            {
                if (memberPerCountry.ContainsKey(member.CountryId))
                {
                    memberPerCountry[member.CountryId]++;
                }
                else
                {
                    memberPerCountry.Add(member.CountryId, 0);
                }
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<string> ids = memberPerCountry.Keys.Select(x => string.Format("'{0}'", x)).ToList();
                string query = string.Format("SELECT id, vkid, title FROM country WHERE vkid IN ({0})", new SeparatedStringBuilder(ids));
                var countries = dataGateway.Connection.Query<Country>(query).ToDictionary(x => int.Parse(x.VkId));

                IList<GroupedObject<Country>> items = memberPerCountry.Select(item => new GroupedObject<Country>
                {
                    Item = countries.ContainsKey(item.Key) ? countries[item.Key] : new Country { Id = 0, VkId = item.Key.ToString(), Title = "<неизвестно>" },
                    Value = item.Value
                }).ToList();

                return items;
            }
        }
        public IEnumerable<GroupedObject<City>> GetCityInformationActiveUsers(int vkGroupId, AudienceFilteringResult result)
        {
            IDictionary<int, int> memberPerCity = new Dictionary<int, int>();

            foreach (var member in result.Members)
            {
                if (memberPerCity.ContainsKey(member.CityId))
                {
                    memberPerCity[member.CityId]++;
                }
                else
                {
                    memberPerCity.Add(member.CityId, 0);
                }
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IList<string> ids = memberPerCity.Keys.Select(x => string.Format("'{0}'", x)).ToList();
                string query = string.Format("SELECT id, vkid, title FROM city WHERE vkid IN ({0})", new SeparatedStringBuilder(ids));
                var cities = dataGateway.Connection.Query<City>(query).ToDictionary(x => int.Parse(x.VkId));

                IList<GroupedObject<City>> items = memberPerCity.Select(item => new GroupedObject<City>
                {
                    Item = cities.ContainsKey(item.Key) ? cities[item.Key] : new City { Id = 0, VkId = item.Key.ToString(), Title = "<неизвестно>" },
                    Value = item.Value
                }).ToList();

                return items;
            }
        }

        public CategorialValue LikesRepostCommentDiagramData(int vkGroupId, AudienceFilteringResult filteringResult)
        {
            var result = new CategorialValue();
            string userIds = filteringResult.IsAllMembers
                ? string.Empty
                : QueryArrayBuilder.GetString(filteringResult.Members.Select(m => m.VkMemberId).ToArray());

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string creatorIdClause = filteringResult.IsAllMembers
                    ? string.Empty
                    : string.Format("AND creatorid = ANY({0})", userIds);

                string memberIdClause = filteringResult.IsAllMembers
                    ? string.Empty
                    : string.Format("AND vkmemberid = ANY({0})", userIds);

                string query = string.Format("SELECT COUNT(*)::integer FROM postcomment WHERE vkgroupid = {0} {1}", vkGroupId, creatorIdClause);
                result.CategoryA = dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COUNT(*)::integer FROM membershare WHERE vkgroupid = {0} {1} AND itemtype = {2}", vkGroupId, memberIdClause, (int)LikeShareType.Post);
                result.CategoryB = dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COUNT(*)::integer FROM memberlike WHERE vkgroupid = {0} {1} AND itemtype = {2}", vkGroupId, memberIdClause, (int)LikeShareType.Post);
                result.CategoryC = dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COUNT(*)::integer FROM post WHERE vkgroupid = {0} {1}", vkGroupId, creatorIdClause);
                result.CategoryD = dataGateway.Connection.Query<int>(query).SingleOrDefault();
            }

            return result;
        }
        public CategorialValue GetTypeOfContentDiagramData(int vkGroupId, AudienceFilteringResult filteringResult)
        {
            var result = new CategorialValue();
            string userIds = filteringResult.IsAllMembers
                ? string.Empty
                : QueryArrayBuilder.GetString(filteringResult.Members.Select(m => m.VkMemberId).ToArray());

            int postCount;
            int photoCount;
            int videoCount;

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string creatorIdClause = filteringResult.IsAllMembers
                    ? string.Empty
                    : string.Format("AND creatorid = ANY({0})", userIds);

                string memberIdClause = filteringResult.IsAllMembers
                    ? string.Empty
                    : string.Format("AND vkmemberid = ANY({0})", userIds);

                string query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM postcomment WHERE vkgroupid = {0} {1} GROUP BY vkpostid) AS sub", vkGroupId, creatorIdClause);
                postCount = dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM memberlike WHERE vkgroupid = {0} {1} AND itemtype = 1 GROUP BY itemid) AS sub", vkGroupId, memberIdClause);
                postCount += dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM membershare WHERE vkgroupid = {0} {1} AND itemtype = 1 GROUP BY itemid) AS sub", vkGroupId, memberIdClause);
                postCount += dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM videocomment WHERE vkgroupid = {0} {1} GROUP BY vkvideoid) AS sub", vkGroupId, creatorIdClause);
                var nullableVideoCount = dataGateway.Connection.Query<int?>(query).SingleOrDefault();
                videoCount = nullableVideoCount.HasValue ? (int)nullableVideoCount : 0;

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM memberlike WHERE vkgroupid = {0} {1} AND itemtype = 4 GROUP BY itemid) AS sub", vkGroupId, memberIdClause);
                videoCount += dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM membershare WHERE vkgroupid = {0} {1} AND itemtype = 4 GROUP BY itemid) AS sub", vkGroupId, memberIdClause);
                videoCount += dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM memberlike WHERE vkgroupid = {0} {1} AND itemtype = 3 GROUP BY itemid) AS sub", vkGroupId, memberIdClause);
                photoCount = dataGateway.Connection.Query<int>(query).SingleOrDefault();

                query = string.Format("SELECT COALESCE(SUM(sub.count)::integer, 0) FROM (SELECT COUNT(*) FROM membershare WHERE vkgroupid = {0} {1} AND itemtype = 3 GROUP BY itemid) AS sub", vkGroupId, memberIdClause);
                photoCount += dataGateway.Connection.Query<int>(query).SingleOrDefault();
            }

            result.CategoryA = postCount;
            result.CategoryB = videoCount;
            result.CategoryC = photoCount;

            return result;
        }

        public IList<MemberSubscriptionInfo> GetMemberSubscriptionInfo(int vkGroupId, AudienceFilteringResult filteringResult, int limit = 15)
        {
            string userIds = filteringResult.IsAllMembers
                ? string.Empty
                : QueryArrayBuilder.GetString(filteringResult.Members.Select(m => m.VkMemberId).ToArray());

            string memberIdClause = filteringResult.IsAllMembers
                ? string.Empty
                : string.Format("AND vkmemberid = ANY({0})", userIds);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                string query = string.Format(
@"with populargroups AS
(
SELECT 
	subscribedvkgroupid,
	COUNT(*)::integer 
  FROM membersubscriptions 
  WHERE vkgroupid = {0} AND 
  subscribedvkgroupid <> {0}
  {1}   
  GROUP BY subscribedvkgroupid 
  ORDER BY count DESC LIMIT {2}
),
select distinct
  vkr.namegroup,
  vkr.screenname,
  vkr.vkgroupid,
  vkr.photo,
  pg.count
from populargroups pg 
inner join vkgroupreference vkr on (pg.subscribedvkgroupid = vkr.vkgroupid)", 
                         vkGroupId, 
                         memberIdClause, 
                         limit);
                return dataGateway.Connection.Query<MemberSubscriptionInfo>(query).ToList();
            }
        }
    }
}