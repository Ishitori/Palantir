namespace Ix.Palantir.Services.API.Analytics
{
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API.Metrics;

    public class RankedModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }

        public RankedValue<double> AverageCommentsPerPost { get; set; }
        public RankedValue<double> AverageLikesPerPost { get; set; }
        public RankedValue<double> InteractionRate { get; set; }
        public RankedValue<double> ResponseRate { get; set; }
        public RankedValue<double> InvolmentRate { get; set; }
        public RankedValue<string> ResponseTime { get; set; }
        public RankedValue<double> UgcRate { get; set; }

        public RankedValue<int> PhotosCount { get; set; }
        public RankedValue<int> VideosCount { get; set; }
        public RankedValue<int> PostsCount { get; set; }
        public RankedValue<int> TopicsCount { get; set; }
        public RankedValue<int> TopicCommentsCount { get; set; }
        public RankedValue<double> SharePerPost { get; set; }

        public RankedValue<int> UsersPostCount { get; set; }
        public RankedValue<int> AdminPostCount { get; set; }

        public RankedValue<double> UsersPostsPerUser { get; set; }
        public RankedValue<double> AdminPostsPerAdmin { get; set; }

        public PostDensity PostBiggestActivities { get; set; }

        public int? SameUsersCount { get; set; }
        public RankedValue<int> UsersCount { get; set; }
        public RankedValue<int> BadFans { get; set; }
        public RankedValue<int> BotsCount { get; set; }
        public RankedValue<int> InactiveUsersCount { get; set; }
        public RankedValue<int> ActiveUsersCount { get; set; }

        public static RankedModel Create(DashboardMetrics initialMetrics)
        {
            var model = new RankedModel();

            model.ProjectId = initialMetrics.Project.Id;
            model.ProjectTitle = initialMetrics.Project.Title;

            model.AverageCommentsPerPost = new RankedValue<double>(initialMetrics.AverageCommentsPerPost);
            model.AverageLikesPerPost = new RankedValue<double>(initialMetrics.AverageLikesPerPost);
            model.InteractionRate = new RankedValue<double>(initialMetrics.InteractionRate);
            model.ResponseRate = new RankedValue<double>(initialMetrics.ResponseRate);
            model.ResponseTime = new RankedValue<string>(initialMetrics.ResponseTime);
            model.InvolmentRate = new RankedValue<double>(initialMetrics.InvolmentRate);
            model.UgcRate = new RankedValue<double>(initialMetrics.UgcRate);

            model.PhotosCount = new RankedValue<int>(initialMetrics.PhotosCount);
            model.VideosCount = new RankedValue<int>(initialMetrics.VideosCount);
            model.PostsCount = new RankedValue<int>(initialMetrics.PostsCount);
            model.TopicsCount = new RankedValue<int>(initialMetrics.TopicsCount);
            model.TopicCommentsCount = new RankedValue<int>(initialMetrics.TopicCommentsCount);
            model.SharePerPost = new RankedValue<double>(initialMetrics.SharePerPost);

            model.UsersPostCount = new RankedValue<int>(initialMetrics.UsersPostCount);
            model.AdminPostCount = new RankedValue<int>(initialMetrics.AdminPostCount);

            model.UsersPostsPerUser = new RankedValue<double>(initialMetrics.UsersPostsPerUser);
            model.AdminPostsPerAdmin = new RankedValue<double>(initialMetrics.AdminPostsPerAdmin);

            model.PostBiggestActivities = initialMetrics.PostBiggestActivities;

            model.UsersCount = new RankedValue<int>(initialMetrics.UsersCount);
            model.BadFans = new RankedValue<int>(initialMetrics.BadFans);
            model.BotsCount = new RankedValue<int>(initialMetrics.BotsCount);
            model.InactiveUsersCount = new RankedValue<int>(initialMetrics.InactiveUsersCount);
            model.ActiveUsersCount = new RankedValue<int>(initialMetrics.ActiveUsersCount);

            return model;
        }
    }
}