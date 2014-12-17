namespace Ix.Palantir.UI.Models.Settings
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Models.Metrics;

    public class GroupSettingsViewModel : MetricsViewModel
    {
        public GroupSettingsViewModel(MetricsBase metrics, IEnumerable<GroupProcessingItem> history, bool canDeleteProjects) : base(metrics)
        {
            this.CanDeleteGroup = canDeleteProjects;
            this.History = new List<GroupSettingsViewModelItem>();

            foreach (var item in history)
            {
                this.History.Add(new GroupSettingsViewModelItem(item));       
            }
        }

        public bool CanDeleteGroup { get; set; }
        public IList<GroupSettingsViewModelItem> History { get; set; }
    }
}