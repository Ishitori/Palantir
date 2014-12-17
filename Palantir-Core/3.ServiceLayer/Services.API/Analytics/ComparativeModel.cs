namespace Ix.Palantir.Services.API.Analytics
{
    using Ix.Palantir.Querying.Common;

    public class ComparativeModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }

        public DoubleComparativeValue AverageCommentsPerPost { get; set; }
        public DoubleComparativeValue AverageLikesPerPost { get; set; }
        public DoubleComparativeValue InteractionRate { get; set; }
        public DoubleComparativeValue ResponseRate { get; set; }
        public DoubleComparativeValue InvolmentRate { get; set; }
        public DoubleComparativeValue UgcRate { get; set; }

        public Int32ComparativeValue PhotosCount { get; set; }
        public Int32ComparativeValue VideosCount { get; set; }
        public Int32ComparativeValue PostsCount { get; set; }
        public Int32ComparativeValue TopicsCount { get; set; }
        public Int32ComparativeValue TopicCommentsCount { get; set; }

        public Int32ComparativeValue UsersPostCount { get; set; }
        public Int32ComparativeValue AdminPostCount { get; set; }

        public DoubleComparativeValue UsersPostsPerUser { get; set; }
        public DoubleComparativeValue AdminPostsPerAdmin { get; set; }

        public PostDensity PostBiggestActivities { get; set; }

        public int SameUsersCount { get; set; }
        public Int32ComparativeValue UsersCount { get; set; }
        public Int32ComparativeValue BadFans { get; set; }
        public Int32ComparativeValue BotsCount { get; set; }
        public Int32ComparativeValue InactiveUsersCount { get; set; }
        public Int32ComparativeValue ActiveUsersCount { get; set; }
    }
}