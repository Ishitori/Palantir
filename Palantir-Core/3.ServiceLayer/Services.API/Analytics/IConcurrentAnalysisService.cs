namespace Ix.Palantir.Services.API.Analytics
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;

    public interface IConcurrentAnalysisService
    {
        ConcurrentAnalysisIntroModel GetConcurrentAnalysisIntro(int projectId);
        ConcurrentAnalysisModel GetConcurrentAnalysis(int projectId, IList<int> concurrentIds, DateRange dateRange);
        void UpdateProjectsConcurrents(int projectId, IList<int> concurrentIds);
    }
}