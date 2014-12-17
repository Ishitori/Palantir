namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Models.Metrics;
    using Models.Converters;
    using Querying.Common.DataFilters;

    [Authorize]
    public class PostsController : MetricsController
    {
        private readonly IMetricsService metricsService;

        public PostsController(IMetricsService metricsService)
        {
            this.metricsService = metricsService;
        }

        public override ActionResult Index(int id)
        {
            PostsMetrics metrics = this.metricsService.GetPostsMetrics(id);
            MetricsViewModel viewModel = new PostsViewModel(metrics);

            return this.MetricsView("Posts", viewModel);
        }
        public ActionResult GetMostPopularPosts(int id, DataFilter filter)
        {
            var converter = new UiTableModelsConverter();
            IList<PostEntityInfo> postEntities = this.metricsService.GetMostPopularPosts(id, DateRangeConverter.GetDateRange(filter));
            return this.MetricsView("_MostPopularPosts", postEntities.Select(converter.CreatePostModel).ToList());
        }
    }
}