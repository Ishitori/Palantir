namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Services.API.Metrics;

    public class AudienceViewModel : MetricsViewModel
    {
        public AudienceViewModel(MetricsBase metrics, IList<SelectListItem> cities, string filter) : base(metrics)
        {
            this.Cities = cities;
            this.Filter = filter;
        }

        public IList<SelectListItem> Cities { get; set; }
        public string Filter { get; set; }
    }
}