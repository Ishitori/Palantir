namespace Ix.Palantir.Services.API.Metrics
{
    using System.Collections.Generic;

    public class PostsMetrics : MetricsBase
    {
        public IList<PostEntityInfo> MostPopularPosts { get; set; }
    }
}