namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Analytics;

    public class CompareGroupsInitViewModel : MetricsViewModel
    {
        public CompareGroupsInitViewModel(ConcurrentAnalysisIntroModel metrics) : base(metrics)
        {
            this.ConcurrentIds = metrics.ConcurrentIds;
        }

        public int TotalProjectsForComparisonCount
        {
            get { return this.ConcurrentIds.Count + 1; }
        }
        public IList<int> ConcurrentIds { get; private set; }
        public IList<Project> PossibleConcurrents
        {
            get { return this.ProjectList.Where(p => p.Id != this.Project.Id).ToList(); }
        }
        public bool IsConcurrentSelected(Project project)
        {
            return this.ConcurrentIds.Any(c => c == project.Id);
        }
    }
}