namespace Ix.Palantir.Services.API.Metrics
{
    using System.Collections.Generic;

    public class UserMetrics : MetricsBase
    {
        public IList<ActiveUserInfo> MostActiveUsers { get; set; }
    }
}
