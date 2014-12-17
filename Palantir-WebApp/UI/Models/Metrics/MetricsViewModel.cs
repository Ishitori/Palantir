namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;

    public class MetricsViewModel
    {
        public MetricsViewModel(MetricsBase metrics, CheckAvailabilityModel checkAvailability = null)
        {
            this.Project = metrics.Project;
            this.ProjectList = metrics.ProjectList;
            this.CheckAvailability = checkAvailability;
            this.FirstPostDate = metrics.FirstPostDate.HasValue ? metrics.FirstPostDate.Value.ToString(string.Empty) : "<неизвестно>";
        }

        public Project Project { get; private set; }
        public IEnumerable<Project> ProjectList { get; private set; }
        public string FirstPostDate { get; private set; }
        public CheckAvailabilityModel CheckAvailability { get; set; }
    }
}