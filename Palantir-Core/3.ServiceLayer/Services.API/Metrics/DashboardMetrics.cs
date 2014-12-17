namespace Ix.Palantir.Services.API.Metrics
{
    using System;
    using System.Collections.Generic;

    public class DashboardMetrics : MetricsBase
    {
        public double AverageCommentsPerPost { get; set; }
        public double AverageLikesPerPost { get; set; }
        public double InteractionRate { get; set; }
        public double ResponseRate { get; set; }
        public string ResponseTime { get; set; }
        public double InvolmentRate { get; set; }
        public double UgcRate { get; set; }

        public int PhotosCount { get; set; }
        public int VideosCount { get; set; }
        public int PostsCount { get; set; }
        public int TopicsCount { get; set; }
        public int TopicCommentsCount { get; set; }

        public DateTime? LastPostDate { get; set; }

        public int UsersPostCount { get; set; }
        public int AdminPostCount { get; set; }

        public double UsersPostsPerUser { get; set; }
        public double AdminPostsPerAdmin { get; set; }
        public double SharePerPost { get; set; }

        public PostDensity PostBiggestActivities { get; set; }

        public IList<long> AllUserIds { get;  set; }

        public int UsersCount { get; set; }
        public int BadFans { get; set; }
        public int BotsCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int InactiveUsersCount { get; set; }
    }
}