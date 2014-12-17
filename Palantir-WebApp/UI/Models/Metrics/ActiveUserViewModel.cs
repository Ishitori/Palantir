namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Models.Shared;
    using Services.API.Metrics;

    public class ActiveUserViewModel : MetricsViewModel
    {
        public ActiveUserViewModel(UserMetrics metrics) : base(metrics)
        {
            Contract.Requires(metrics != null);

            this.MostActiveUsers = new List<MostActiveUsers>();
            
            foreach (var user in metrics.MostActiveUsers)
            {
                this.MostActiveUsers.Add(this.CreateUser(user));
            }
        }

        public IList<MostActiveUsers> MostActiveUsers { get; private set; }

        private MostActiveUsers CreateUser(ActiveUserInfo user)
        {
            return new MostActiveUsers
            {
                Name = new UiLink { Target = "_blank", Text = user.Name, Url = user.Url },
                NumberOfPosts = user.NumberOfPosts,
                NumberOfComments = user.NumberOfComments,
                NumberOfLikes = user.NumberOfLikes,
                NumberOfShare = user.NumberOfShares,
                Sum = user.Sum
            };
        }
    }
}