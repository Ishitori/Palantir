namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;

    public class StatisticsCalculator
    {
        private readonly int postsCount;
        private readonly int postLikesCount;
        private readonly int postCommentsCount;
        private readonly int adminPostsCount;
        private readonly int postsWithAdminCommentsCount;
        private readonly int membersCount;
        private readonly int shareCount;

        public StatisticsCalculator(
            int postsCount,
            int postLikesCount,
            int postCommentsCount,
            int shareCount,
            int adminPostsCount,
            int postsWithAdminCommentsCount,
            int membersCount)
        {
            this.postsCount = postsCount;
            this.postLikesCount = postLikesCount;
            this.postCommentsCount = postCommentsCount;
            this.adminPostsCount = adminPostsCount;
            this.postsWithAdminCommentsCount = postsWithAdminCommentsCount;
            this.membersCount = membersCount;
            this.shareCount = shareCount;
        }

        public Kpi CalculateKpis()
        {
            var calculator = new InteractionRateCalculator();

            var kpi = new Kpi();
            kpi.UsersCount = this.membersCount;

            if (this.postsCount <= 0)
            {
                return kpi;
            }

            kpi.PostsCount = this.postsCount;
            kpi.AverageLikesPerPost = (double)this.postLikesCount / this.postsCount;
            kpi.AverageCommentsPerPost = (double)this.postCommentsCount / this.postsCount;
            kpi.InteractionRate = this.membersCount > 0 ? calculator.GetInteractionRate(this.postCommentsCount, this.postLikesCount, this.postsCount, this.shareCount, this.membersCount) : 0;
            kpi.ResponseRate = (double)this.postsWithAdminCommentsCount / this.postsCount;
            kpi.UgcRate = (double)(this.postsCount - this.adminPostsCount) / this.postsCount;
            kpi.InvolmentRate = this.membersCount > 0 ? (double)this.postsCount / this.membersCount : 0;

            kpi.AdminPostsCount = this.adminPostsCount;
            kpi.UserPostsCount = this.postsCount - this.adminPostsCount;
            kpi.SharePerPosts = (double)this.shareCount / this.postsCount;

            return kpi;
        }
    }
}