namespace Ix.Palantir.Services.API.Metrics
{
    using System.Collections.Generic;

    public class SocialMetrics : MetricsBase
    {
        public IList<PopularCityInfo> MostPopularCities { get; set; }
        public int MembersWithoutCity { get; set; }
    }
}