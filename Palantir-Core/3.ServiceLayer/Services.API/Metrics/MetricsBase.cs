namespace Ix.Palantir.Services.API.Metrics
{
    using System;
    using System.Collections.Generic;

    public class MetricsBase
    {
        public Project Project { get; set; }
        public IEnumerable<Project> ProjectList { get; set; }
        public DateTime? FirstPostDate { get; set; }
    }
}