namespace Ix.Palantir.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;

    public class MemberAdvancedSearchService : IMemberAdvancedSearchService
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IProjectRepository projectRepository;
        private readonly IMemberAdvancedSearcher searcher;
        private readonly IStatisticsProvider statisticsProvider;
        private readonly IMemberAdvancedSearchCache cacher;

        public MemberAdvancedSearchService(
            IUnitOfWorkProvider unitOfWorkProvider, 
            IProjectRepository projectRepository, 
            IMemberAdvancedSearcher searcher, 
            IStatisticsProvider statisticsProvider,
            IMemberAdvancedSearchCache cacher)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.projectRepository = projectRepository;
            this.statisticsProvider = statisticsProvider;
            this.searcher = searcher;
            this.cacher = cacher;
        }

        public int SearchAudience(int projectId, AudienceFilter filter)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var result = this.GetFilteringResult(vkGroup, filter.Code, throwException: false);

                if (result != null)
                {
                    return result.Members.Count;
                }

                result = new AudienceFilteringResult(filter.Code);

                if (filter.IsEmpty)
                {
                    result.IsAllMembers = true;
                }

                result.Members = this.searcher.GetAudienceByFilter(projectId, filter);
                this.cacher.SetToCache(vkGroup.Id, result);
                return result.Members.Count;
            }
        }

        public CategorialValue GetAgeActiveUsers(int projectId, long filterCode, bool withoutMonthAndDay = false)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);

                return this.searcher.GetAgeInformationActiveUsers(vkGroup.Id, filteringResult, withoutMonthAndDay);
            }
        }
        public CategorialValue GetAgeActiveUsers(int projectId, IList<ActiveUserInfo> users, bool withoutMonthAndDay = false)
        {
            IList<MemberMainInfo> members = this.ToMemberMainInfo(users);

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = new AudienceFilteringResult(members);

                return this.searcher.GetAgeInformationActiveUsers(vkGroup.Id, filteringResult, withoutMonthAndDay);
            }
        }

        public GenderInformation GetGenderInformationActiveUsers(int projectId, long filterCode)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);

                CategorialValue categories = this.searcher.GetGenderInformationActiveUsers(vkGroup.Id, filteringResult);
                return GenderInformation.Create(categories);
            }
        }
        public GenderInformation GetGenderInformationActiveUsers(int projectId, IList<ActiveUserInfo> users)
        {
            IList<MemberMainInfo> members = this.ToMemberMainInfo(users);

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = new AudienceFilteringResult(members);

                CategorialValue categories = this.searcher.GetGenderInformationActiveUsers(vkGroup.Id, filteringResult);
                return GenderInformation.Create(categories);
            }
        }

        public EducationLevelInformation GetEducationLevelInformationActiveUsers(int projectId, long filterCode)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);

                CategorialValue categories = this.searcher.GetEducationLevelInformationActiveUsers(vkGroup.Id, filteringResult);
                return EducationLevelInformation.Create(categories);
            }
        }
        public EducationLevelInformation GetEducationLevelInformationActiveUsers(int projectId, IList<ActiveUserInfo> users)
        {
            IList<MemberMainInfo> members = this.ToMemberMainInfo(users);

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = new AudienceFilteringResult(members);

                CategorialValue categories = this.searcher.GetEducationLevelInformationActiveUsers(vkGroup.Id, filteringResult);
                return EducationLevelInformation.Create(categories);
            }
        }

        public IEnumerable<PopularCityInfo> GetMostPopularCities(int projectId, long filterCode, int count)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                IList<PopularCityInfo> cityInfos = new List<PopularCityInfo>();

                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);
                var cityCountryMapping = new Dictionary<int, int>();

                foreach (var member in filteringResult.Members)
                {
                    if (!cityCountryMapping.ContainsKey(member.CityId))
                    {
                        cityCountryMapping.Add(member.CityId, member.CountryId);
                    }
                }

                var countries = this.searcher.GetCountryInformationActiveUsers(vkGroup.Id, filteringResult).ToDictionary(x => int.Parse(x.Item.VkId));
                var cities = this.searcher.GetCityInformationActiveUsers(vkGroup.Id, filteringResult).ToList();

                foreach (var city in cities.Where(c => c.Item.Id != 0))
                {
                    var cityInfo = new PopularCityInfo()
                    {
                        City = city.Item.Title,
                        Country = countries[cityCountryMapping[int.Parse(city.Item.VkId)]].Item.Title,
                        MembersCount = city.Value
                    };

                    cityInfos.Add(cityInfo);
                }

                var popularCityInfos = cityInfos.OrderByDescending(c => c.MembersCount).Take(count).ToList();
                return popularCityInfos;
            }
        }
        public LRCDiagramDataInfo GetLikesRepostCommentDiagramData(int projectId, long filterCode)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);

                CategorialValue diagramData = this.searcher.LikesRepostCommentDiagramData(vkGroup.Id, filteringResult);
                return LRCDiagramDataInfo.Create(diagramData);
            }
        }

        public TypeOfContentDataInfo GetTypeOfContentDiagramData(int projectId, long filterCode)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);

                CategorialValue diagramData = this.searcher.GetTypeOfContentDiagramData(vkGroup.Id, filteringResult);
                return TypeOfContentDataInfo.Create(diagramData);
            }
        }

        public IList<MemberInterestsObject> GetMemberInterests(int projectId, long filterCode, int count)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);
                long[] userIds = filteringResult.Members.Select(m => m.VkMemberId).ToArray();

                List<MemberInterestsGroupedObject> interesets = count > 0
                    ? this.statisticsProvider.GetMemberInterest(projectId, userIds, count).ToList()
                    : this.statisticsProvider.GetMemberInterest(projectId, userIds).ToList();

                return interesets.Select(x => new MemberInterestsObject()
                {
                    Title = x.Title,
                    Type = ((MemberInterestType)x.Type).GetTile(),
                    Count = x.Count
                }).ToList();
            }
        }
        public IList<MemberSubInfo> MemberSubInfos(int projectId, long filterCode, int limit = 15)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var filteringResult = this.GetFilteringResult(vkGroup, filterCode);

                var memberSubscriptionInfos = this.searcher.GetMemberSubscriptionInfo(vkGroup.Id, filteringResult, limit);
                return memberSubscriptionInfos.Select(x => new MemberSubInfo()
                {
                    Url = string.Format("http://vk.com/{0}", x.ScreenName),
                    Name = x.NameGroup,
                    Image = x.Photo,
                    Count = x.Count
                }).ToList();
            }
        }

        private AudienceFilteringResult GetFilteringResult(VkGroup vkGroup, long filterCode, bool throwException = true)
        {
            var filteringResult = this.cacher.GetFromCache(vkGroup.Id, filterCode);

            if (filteringResult == null)
            {
                if (throwException)
                {
                    throw new ArgumentException(string.Format("Unable to find search result by code \"{0}\"", filterCode), "filterCode");
                }

                return null;
            }

            return filteringResult;
        }
        private IList<MemberMainInfo> ToMemberMainInfo(IEnumerable<ActiveUserInfo> activeUsers)
        {
            IList<MemberMainInfo> members = new List<MemberMainInfo>();

            foreach (var user in activeUsers)
            {
                var member = new MemberMainInfo
                {
                    VkMemberId = user.Id,
                    BirthDate = new BirthDate(user.BirthDate.BirthYear, user.BirthDate.BirthMonth, user.BirthDate.BirthDay),
                    CityId = user.CityId,
                    CountryId = user.CountryId,
                    Education = (EducationLevel)(int)user.Education,
                    Gender = (Gender)(int)user.Gender
                };

                members.Add(member);
            }

            return members;
        }
    }
}