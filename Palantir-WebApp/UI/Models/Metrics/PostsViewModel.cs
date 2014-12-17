namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using Converters;
    using Services.API.Metrics;

    public class PostsViewModel : MetricsViewModel
    {
        public PostsViewModel(PostsMetrics metrics) : base(metrics)
        {
            var converter = new UiTableModelsConverter();
            this.MostPopularPosts = metrics.MostPopularPosts.Select(converter.CreatePostModel).ToList();
        }

        public IList<MostPopularPostModel> MostPopularPosts
        {
            get;
            private set;
        }
    }
}