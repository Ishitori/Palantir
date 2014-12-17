namespace Ix.Palantir.Services.API.Analytics
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API.Metrics;

    public class ConcurrentAnalysisIntroModel : MetricsBase
    {
        public ConcurrentAnalysisIntroModel()
        {
            this.ConcurrentIds = new List<int>();
        }

        public IList<int> ConcurrentIds { get; set; }
    }
}