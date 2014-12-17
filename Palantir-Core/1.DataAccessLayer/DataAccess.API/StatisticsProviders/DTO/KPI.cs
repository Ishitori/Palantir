namespace Ix.Palantir.DataAccess.API.StatisticsProviders.DTO
{
    public class Kpi
    {
        public int UsersCount { get; set; }
        public int PostsCount { get; set; }
        public int UserPostsCount { get; set; }
        public int AdminPostsCount { get; set; }

        public double SharePerPosts { get; set; }
        public double UserPostsPerUser { get; set; }
        public double AdminPostsPerAdmin { get; set; }

        public double AverageLikesPerPost { get; set; }
        public double AverageCommentsPerPost { get; set; }
        public double InteractionRate { get; set; }
        public double ResponseRate { get; set; }
        public double InvolmentRate { get; set; }
        public double UgcRate { get; set; }
    }
}