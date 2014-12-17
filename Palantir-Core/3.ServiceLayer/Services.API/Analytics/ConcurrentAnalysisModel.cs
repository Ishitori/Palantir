namespace Ix.Palantir.Services.API.Analytics
{
    using System.Collections.Generic;

    public class ConcurrentAnalysisModel
    {
        private readonly RankedModel baseMetrics;

        public ConcurrentAnalysisModel()
        {
            this.Comparisons = new List<RankedModel>();
        }
        public ConcurrentAnalysisModel(RankedModel baseMetrics, IList<RankedModel> concurrentsRankedMetrics)
        {
            this.baseMetrics = baseMetrics;
            this.Comparisons = concurrentsRankedMetrics;
        }

        public RankedModel BaseMetrics
        {
            get { return this.baseMetrics; }
        }
        public IList<RankedModel> Comparisons { get; private set; }
    }
}