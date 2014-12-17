namespace Ix.Palantir.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DomainModel;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Analytics;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UrlManagement.API;
    using Querying.Common;
    using Utilities;
    using Project = Ix.Palantir.Services.API.Project;

    public class MetricsService : IMetricsService
    {
        private readonly IProjectService projectService;
        private readonly IAnalyticsService analyticsService;
        private readonly IStatisticsProvider statisticsProvider;
        private readonly IKpiProvider kpiProvider;
        private readonly IVkUrlProvider vkUrlProvider;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IProjectRepository projectRepository;
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IIrChartDataProvider irChartDataProvider;
        private readonly IRawDataProvider rawDataRepository;
        private readonly IVkGroupRepository vkGroupRepository;

        public MetricsService(
            IProjectService projectService,
            IAnalyticsService analyticsService,
            IStatisticsProvider statisticsProvider,
            IKpiProvider kpiProvider,
            IVkUrlProvider vkUrlProvider,
            IDateTimeHelper dateTimeHelper,
            IProjectRepository projectRepository,
            IUnitOfWorkProvider unitOfWorkProvider,
            IIrChartDataProvider irChartDataProvider,
            IRawDataProvider rawDataRepository,
            IVkGroupRepository vkGroupRepository)
        {
            this.projectService = projectService;
            this.analyticsService = analyticsService;
            this.statisticsProvider = statisticsProvider;
            this.kpiProvider = kpiProvider;
            this.vkUrlProvider = vkUrlProvider;
            this.dateTimeHelper = dateTimeHelper;
            this.projectRepository = projectRepository;
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.irChartDataProvider = irChartDataProvider;
            this.rawDataRepository = rawDataRepository;
            this.vkGroupRepository = vkGroupRepository;
        }

        public IDictionary<int, string> GetCities(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var cities = this.statisticsProvider.GetCities(projectId).OrderBy(x => x.Title);
                var result = cities.ToDictionary(x => int.Parse(x.VkId), x => x.Title);

                return result;
            }
        }

        /// <summary>
        /// Получить общую информацию по метрикам.
        /// </summary>
        public MetricsBase GetBaseMetrics(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Project project = this.projectService.GetProject(projectId);
                IEnumerable<Project> projectList = this.projectService.GetProjects();
                var firstPostDate = this.statisticsProvider.GetFirstPostDate(projectId);
                return new MetricsBase
                           {
                               Project = project,
                               ProjectList = projectList,
                               FirstPostDate = firstPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(firstPostDate.Value) : (DateTime?)null
                           };
            }
        }

        public DashboardMetrics GetDashboardMetrics(int projectId, DateRange dateRange)
        {
            dateRange.To = dateRange.To.AddDays(1);
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Project project = this.projectService.GetProject(projectId);
                IEnumerable<Project> projectList = this.projectService.GetProjects();

                var postMostCrowdedTime = this.analyticsService.GetPostMostCrowdedTime(projectId, dateRange);
                var userStatistics = this.analyticsService.GetInactiveUsersCount(projectId, dateRange);

                Kpi kpi = this.kpiProvider.GetKpis(projectId, dateRange);

                var averageLikesPerPost = kpi.AverageLikesPerPost;
                var interactionRate = kpi.InteractionRate;
                var responseRate = kpi.ResponseRate;
                var responseTime = this.GetResponseTime(projectId, dateRange);
                var involmentRate = kpi.InvolmentRate;
                var ugcRate = kpi.UgcRate;
                var postsCount = kpi.PostsCount;
                var averageCommentsPerPost = kpi.AverageCommentsPerPost;
                var usersPostCount = kpi.UserPostsCount;
                var adminPostCount = kpi.AdminPostsCount;
                var usersPostsPerUser = kpi.UserPostsPerUser;
                var adminPostsPerAdmin = kpi.AdminPostsPerAdmin;
                var usersCount = kpi.UsersCount;

                var photosCount = this.statisticsProvider.GetPhotosCount(projectId, dateRange);
                var videosCount = this.statisticsProvider.GetVideosCount(projectId, dateRange);
                var topicsCount = this.statisticsProvider.GetTopicsCount(projectId, dateRange);
                var topicCommentsCount = this.statisticsProvider.GetTopicCommentsCount(projectId, dateRange);
                var firstPostDate = this.statisticsProvider.GetFirstPostDate(projectId);
                var lastPostDate = this.statisticsProvider.GetLastPostDate(projectId);

                return new DashboardMetrics
                           {
                               Project = project,
                               ProjectList = projectList,
                               AverageLikesPerPost = Math.Round(averageLikesPerPost, 2),
                               AverageCommentsPerPost = Math.Round(averageCommentsPerPost, 2),
                               InteractionRate = Math.Round(interactionRate, 2),
                               ResponseRate = Math.Round(responseRate, 2),
                               ResponseTime = responseTime,
                               InvolmentRate = Math.Round(involmentRate, 2),
                               UsersCount = usersCount,
                               UsersPostCount = usersPostCount,
                               AdminPostCount = adminPostCount,
                               UsersPostsPerUser = Math.Round(usersPostsPerUser, 2),
                               AdminPostsPerAdmin = Math.Round(adminPostsPerAdmin, 2),
                               UgcRate = Math.Round(ugcRate, 2),
                               PhotosCount = photosCount,
                               VideosCount = videosCount,
                               TopicsCount = topicsCount,
                               TopicCommentsCount = topicCommentsCount,
                               PostsCount = postsCount,
                               FirstPostDate = firstPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(firstPostDate.Value) : (DateTime?)null,
                               LastPostDate = lastPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(lastPostDate.Value) : (DateTime?)null,
                               PostBiggestActivities = postMostCrowdedTime,
                               InactiveUsersCount = userStatistics.InactiveUsers.Count,
                               BadFans = userStatistics.BlockedUsers.Count + userStatistics.DeletedUsers.Count,
                               BotsCount = userStatistics.Bots.Count,
                               ActiveUsersCount = userStatistics.ActiveUsers.Count,
                               AllUserIds = userStatistics.AllUsers,
                               SharePerPost = Math.Round(kpi.SharePerPosts, 2)
                           };
            }
        }

        public PostsMetrics GetPostsMetrics(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Project project = this.projectService.GetProject(projectId);
                IEnumerable<Project> projectList = this.projectService.GetProjects();
                var posts = new List<PostEntityInfo>();
                var firstPostDate = this.statisticsProvider.GetFirstPostDate(projectId);

                return new PostsMetrics
                           {
                               Project = project,
                               ProjectList = projectList,
                               FirstPostDate = firstPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(firstPostDate.Value) : (DateTime?)null,
                               MostPopularPosts = posts
                           };
            }
        }

        public IList<PostEntityInfo> GetMostPopularPosts(int projectId, DateRange dateRange)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                VkGroup vkGroup = this.projectRepository.GetVkGroup(projectId);
                var posts = this.statisticsProvider.GetMostPopularPosts(projectId, PopularBy.LikesAndComments, dateRange);
                return posts.Select(x => this.CreatePostEntityInfo(vkGroup.Id, x)).ToList();
            }
        }

        public IList<ContentEntityInfo> GetMostPopularContent(int projectId, DateRange dateRange)
        {
            int maxCount = 30;

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                VkGroup vkGroup = this.projectRepository.GetVkGroup(projectId);
                var photos = this.statisticsProvider.GetMostPopularPhotos(projectId, PopularBy.LikesAndComments, dateRange, maxCount);
                var videos = this.statisticsProvider.GetMostPopularVideos(projectId, PopularBy.LikesAndComments, dateRange, maxCount);

                var popularContent = this.MergePopularityData(photos, videos, vkGroup, maxCount);
                return popularContent;
            }
        }

        public IEnumerable<PointInTime> GetPhotos(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.statisticsProvider.GetPhotos(projectId, dateRange, periodicity);
            }
        }

        public IEnumerable<PointInTime> GetPosts(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.statisticsProvider.GetPosts(projectId, dateRange, periodicity);
            }
        }

        public IEnumerable<IEnumerable<PointInTime>> GetUsersCount(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.statisticsProvider.GetUsersCount(projectId, dateRange, periodicity);
            }
        }

        public IEnumerable<PointInTime> GetVideos(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.statisticsProvider.GetVideos(projectId, dateRange, periodicity);
            }
        }

        public IEnumerable<IEnumerable<PointInTime>> GetInteractionRate(int projectId, DateRange range, Periodicity periodicity)
        {
            VkGroup vkGroup = this.projectRepository.GetVkGroup(projectId);
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.irChartDataProvider.GetInteractionRate(vkGroup.Id, range, periodicity);
            }
        }

        public IList<IList<GroupedObjectWithPercents<string>>> GetInteractionFrequency(int projectId, int count)
        {
            VkGroup vkGroup = this.projectRepository.GetVkGroup(projectId);
            var userCountInGroup = this.rawDataRepository.GetMembersCount(vkGroup.Id);

            var userList = this.statisticsProvider.GetActiveUsers(projectId, null, 0).ToList();
            var userListActiveCount = userList.Count;

            var strictF = new List<GroupedObjectWithPercents<string>>();
            var f = new List<GroupedObjectWithPercents<string>>();

            if (count != 0)
            {
                f.Add(new GroupedObjectWithPercents<string>("1", userList.Count, userCountInGroup, userListActiveCount));

                var usersMadeOneActionCount = userList.Count(x => (x.CommentCount + x.LikeCount + x.PostCount + x.ShareCount) == 1);
                strictF.Add(new GroupedObjectWithPercents<string>("1", usersMadeOneActionCount, userCountInGroup, userListActiveCount));

                for (var i = 2; i <= count; i++)
                {
                    f.Add(new GroupedObjectWithPercents<string>(i.ToString(), f[i - 2].Value - strictF[i - 2].Value, userCountInGroup, userListActiveCount));

                    var usersMadeActions = userList.Count(x => (x.CommentCount + x.LikeCount + x.PostCount + x.ShareCount) == i);
                    strictF.Add(new GroupedObjectWithPercents<string>(i.ToString(), usersMadeActions, userCountInGroup, userListActiveCount));
                }
            }

            var result = new List<IList<GroupedObjectWithPercents<string>>>();
            result.Add(f.ToList());
            result.Add(strictF.ToList());
            return result;
        }

        public IEnumerable<IEnumerable<PointInTime>> GetMetricData(int projectId, DateRange range, Periodicity periodicity)
        {
            return this.statisticsProvider.GetDashboardDataChart(projectId, range, periodicity);
        }

        public EducationLevelInformation GetEducationLevelInformation(int projectid)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                CategorialValue categories = this.statisticsProvider.GetEducationLevelInformation(projectid);
                return EducationLevelInformation.Create(categories);
            }
        }

        public DateRange PostDateLimit(int projectId)
        {
            var min = this.statisticsProvider.GetFirstPostDate(projectId);
            var max = this.statisticsProvider.GetLastPostDate(projectId);
            if (min == null)
            {
                return null;
            }
            return new DateRange((DateTime)min, (DateTime)max);
        }

        public DateRange MemberDateLimit(int projectId)
        {
            var min = this.statisticsProvider.GetFirstMemberDate(projectId);
            var max = this.statisticsProvider.GetLastMemberDate(projectId);
            
            if (min == null)
            {
                return null;
            }

            return new DateRange((DateTime)min, (DateTime)max);
        }

        public IList<IList<GroupedObjectDouble<string>>> GetLikesCommentsRepostsAverageCount(int projectId, DateRange range)
        {
            ////0 - likes, 1 - comments, 2 - reposts
            float[] p = this.statisticsProvider.GetPostAverageCount(projectId, range).ToArray();
            float[] ph = this.statisticsProvider.GetPhotoAverageCount(projectId, range).ToArray();
            float[] v = this.statisticsProvider.GetVideoAverageCount(projectId, range).ToArray();
            var likes = new List<GroupedObjectDouble<string>>()
                {
                    new GroupedObjectDouble<string>() { Item = "Тексты", Value = p[0] },
                    new GroupedObjectDouble<string>() { Item = "Фотографии", Value = ph[0] }, 
                    new GroupedObjectDouble<string>() { Item = "Видео", Value = v[0] }, 
                };
            var comments = new List<GroupedObjectDouble<string>>()
                {
                    new GroupedObjectDouble<string>() { Item = "Тексты", Value = p[1] },
                    new GroupedObjectDouble<string>() { Item = "Фотографии", Value = ph[1] }, 
                    new GroupedObjectDouble<string>() { Item = "Видео", Value = v[1] }, 
                };
            var reposts = new List<GroupedObjectDouble<string>>()
                {
                    new GroupedObjectDouble<string>() { Item = "Тексты", Value = p[2] },
                    new GroupedObjectDouble<string>() { Item = "Фотографии", Value = ph[2] }, 
                    new GroupedObjectDouble<string>() { Item = "Видео", Value = v[2] }, 
                };

            return new List<IList<GroupedObjectDouble<string>>>() { likes, comments, reposts };
        }

        public GenderInformation GetGenderInformation(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                CategorialValue categories = this.statisticsProvider.GetGenderInformation(projectId);
                return GenderInformation.Create(categories);
            }
        }

        public AgeInformation GetAgeInformation(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                CategorialValue categories = this.statisticsProvider.GetAgeInformation(projectId);
                return AgeInformation.Create(categories);
            }
        }
        public SocialMetrics GetSocialMetrics(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Project project = this.projectService.GetProject(projectId);
                IEnumerable<Project> projectList = this.projectService.GetProjects();
                var firstPostDate = this.statisticsProvider.GetFirstPostDate(projectId);
                IEnumerable<LocationInfoGroupedObject> mostPopularCities = this.statisticsProvider.GetMostPopularCities(projectId);
                int membersWithoutCity = this.statisticsProvider.GetMembersWithoutCityCount(projectId);

                return new SocialMetrics
                {
                    Project = project,
                    ProjectList = projectList,
                    FirstPostDate = firstPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(firstPostDate.Value) : (DateTime?)null,
                    MostPopularCities = mostPopularCities.Select(c => new PopularCityInfo { City = c.City, MembersCount = c.Count, Country = c.Country }).ToList(),
                    MembersWithoutCity = membersWithoutCity
                };
            }
        }

        public IList<PopularCityInfo> GetMostPopularCities(int projectId, long[] usersIds)
        {
            var mostPopularCities = this.statisticsProvider.GetMostPopularCitiesActiveUsers(projectId, usersIds);
            return mostPopularCities.Select(c => new PopularCityInfo { City = c.City, MembersCount = c.Count, Country = c.Country }).ToList();
        }

        public UserMetrics GetUserMetrics(int projectId, DateRange dateRange, int count)
        {
            var result = new UserMetrics()
                {
                    Project = this.projectService.GetProject(projectId),
                    ProjectList = this.projectService.GetProjects(),
                    MostActiveUsers = new List<ActiveUserInfo>()
                };

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                if (dateRange != null)
                {
                    IList<ActiveUser> usersList = this.statisticsProvider.GetActiveUsers(projectId, dateRange, count).ToList();

                    foreach (var user in usersList)
                    {
                        result.MostActiveUsers.Add(new ActiveUserInfo
                        {
                            Id = user.Id,
                            Url = this.vkUrlProvider.GetMemberProfileUrl((int)user.Id),
                            Name = !string.IsNullOrWhiteSpace(user.UserName) ? user.UserName : "<неизвестно>",
                            NumberOfComments = user.CommentCount,
                            NumberOfLikes = user.LikeCount,
                            NumberOfPosts = user.PostCount,
                            NumberOfShares = user.ShareCount,
                            Sum = user.CommentCount + user.LikeCount + user.PostCount + user.ShareCount,
                            BirthDate = new API.DTO.BirthDate(user.BirthDate.BirthYear, user.BirthDate.BirthMonth, user.BirthDate.BirthDay),
                            CityId = user.CityId,
                            CountryId = user.CountryId,
                            Education = (API.DTO.EducationLevel)(int)user.Education,
                            Gender = (API.DTO.Gender)(int)user.Gender
                        });
                    }
                }
            }
            return result;
        }

        public IEnumerable<VkMemberInterest> GetMemberInterests(int projectId, int interestsCount) 
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                IEnumerable<GroupedObject<string>> interests = this.statisticsProvider.GetGroupInterests(projectId, interestsCount);
                return interests.Select(i => new VkMemberInterest { Name = i.Item, MembersCount = i.Value }).ToList();
            }
        }
        public IList<MemberInterestsObject> GetMemberInterests(int projectId, long[] usersId, int count)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                List<MemberInterestsGroupedObject> interesets = count > 0 
                    ? this.statisticsProvider.GetMemberInterest(projectId, usersId, count).ToList() 
                    : this.statisticsProvider.GetMemberInterest(projectId, usersId).ToList();

                return interesets.Select(x => new MemberInterestsObject()
                {
                    Title = x.Title,
                    Type = ((MemberInterestType)x.Type).GetTile(),
                    Count = x.Count
                }).ToList();
            }
        }

        public IEnumerable<VkMemberInfo> GetUsersByInterest(int projectId, string interestTitle) 
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                IEnumerable<NamedEntity> members = this.statisticsProvider.GetMembersByInterest(projectId, interestTitle);
                return members.Select(m => new VkMemberInfo { Id = m.Id, Name = m.Title, Link = this.vkUrlProvider.GetMemberProfileUrl(m.Id) }).ToList();
            }
        }

        public object[] CheckAvailability(int projectId)
        {
             using (this.unitOfWorkProvider.CreateUnitOfWork())
             {
                 VkGroup vkGroup = this.projectRepository.GetVkGroup(projectId);
                 var history = this.vkGroupRepository.GetLatestProcessingItems(vkGroup.Id);

                 int done = history.Count;
                 int total = Enum.GetValues(typeof(DataFeedType)).Cast<DataFeedType>().Count(t => t != DataFeedType.Undefined);

                 return new object[] { done, total, done >= total };
             }
        }

        private PostEntityInfo CreatePostEntityInfo(int vkGroupId, Post post)
        {
            HtmlUtils htmlUtils = new HtmlUtils();
            var postText = htmlUtils.RemoveHtml(post.Text);

            return new PostEntityInfo
                       {
                           Id = post.VkId,
                           PostedDate = this.dateTimeHelper.GetLocalUserDate(post.PostedDate),
                           Title = string.IsNullOrWhiteSpace(postText) ? "Название неизвестно" : postText.FirstSymbols(100),
                           LikesCount = post.LikesCount,
                           CommentsCount = post.CommentsCount,
                           LikesAndCommentsAndShareCount = post.LikesCount + post.CommentsCount + post.SharesCount,
                           ShareCount = post.SharesCount,
                           Url = this.vkUrlProvider.GetPostUrl(vkGroupId.ToString(), post.VkId)
                       };
        }
        private IList<ContentEntityInfo> MergePopularityData(IList<Photo> photos, IList<Video> videos, VkGroup vkGroup, int count)
        {
            IList<ContentEntityInfo> content = new List<ContentEntityInfo>(count);
            int photosIndex = 0;
            int videosIndex = 0;

            for (int i = 0; i < count; i++)
            {
                if (photos.Count > photosIndex && videos.Count > videosIndex)
                {
                    if (photos[photosIndex].CommentsAndLikesAndShareSum > videos[videosIndex].CommentsAndLikesAndShareSum)
                    {
                        var photo = photos[photosIndex];
                        var info = this.ConvertToContentEntity(vkGroup, photo);
                        content.Add(info);
                        photosIndex++;
                    }
                    else
                    {
                        var video = videos[videosIndex];
                        var info = this.ConvertToContentEntity(vkGroup, video);
                        content.Add(info);
                        videosIndex++;
                    }
                }
                else if (photos.Count == photosIndex && videos.Count > videosIndex)
                {
                    var video = videos[videosIndex];
                    var info = this.ConvertToContentEntity(vkGroup, video);
                    content.Add(info);
                    videosIndex++;                
                }
                else if (photos.Count > photosIndex && videos.Count == videosIndex)
                {
                    var photo = photos[photosIndex];
                    var info = this.ConvertToContentEntity(vkGroup, photo);
                    content.Add(info);
                    photosIndex++;
                }
                else
                {
                    return content;
                }
            }

            return content;
        }

        private ContentEntityInfo ConvertToContentEntity(VkGroup vkGroup, Video video)
        {
            var info = new ContentEntityInfo
            {
                Id = video.VkId,
                Title = string.IsNullOrWhiteSpace(video.Title)
                        ? string.Format("Видео {0}", video.VkId)
                        : video.Title,
                PostedDate = this.dateTimeHelper.GetLocalUserDate(video.PostedDate),
                IsVideo = true,
                CommentsCount = video.CommentsCount,
                LikesCount = video.LikesCount,
                ShareCount = video.ShareCount,
                LikesAndCommentsAndShareCount = video.CommentsAndLikesAndShareSum,
                Url = this.vkUrlProvider.GetVideoUrl(vkGroup.Url, video.VkGroupId, video.VkId),
            };
            return info;
        }

        private ContentEntityInfo ConvertToContentEntity(VkGroup vkGroup, Photo photo)
        {
            var info = new ContentEntityInfo
            {
                Id = photo.VkId,
                Title = string.IsNullOrWhiteSpace(photo.Text)
                        ? string.Format("Фотография {0}", photo.VkId)
                        : photo.Text,
                PostedDate = this.dateTimeHelper.GetLocalUserDate(photo.PostedDate),
                IsVideo = false,
                CommentsCount = photo.CommentsCount,
                LikesCount = photo.LikesCount,
                ShareCount = photo.ShareCount,
                LikesAndCommentsAndShareCount = photo.CommentsAndLikesAndShareSum,
                Url = this.vkUrlProvider.GetPhotoUrl(vkGroup.Url, photo.VkGroupId, photo.VkId),
            };
            return info;
        }

        private string GetResponseTime(int projectId, DateRange range)
        {
            long time = this.statisticsProvider.GetAverageResponseTimeInTick(projectId, range) / 600000000;

            if (time > 0)
            {
                if (time < 60)
                {
                    return string.Format("{0} мин", time);
                }

                int day;
                int hour;
                int minute;
            
                if (time < 1440)
                {
                    hour = (int)(time / 60);
                    minute = (int)(time - (60 * hour));
                    return string.Format("{0} ч {1} мин", hour, minute);
                }
                
                day = (int)(time / 1440);
                hour = (int)((time / 60) - (day * 24));
                minute = (int)(time - (1440 * day) - (60 * hour));
                return string.Format("{0} д {1} ч {2} мин", day, hour, minute);
            }
            return "-";
        }
    }
}