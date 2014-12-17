namespace Ix.Palantir.UI.Models.Shared
{
    using System.Collections.Generic;
    using Services.API;

    public class TopEntriesChartModel
    {
        public TopEntriesChartModel(string title, string nameColumnTitle, string valueColumnTitle, IList<EntityInfo> entities)
        {
            this.Title = title;
            this.NameColumnTitle = nameColumnTitle;
            this.ValueColumnTitle = valueColumnTitle;
            this.Entities = entities;
        }

        public string Title { get; set; }
        public string NameColumnTitle { get; set; }
        public string ValueColumnTitle { get; set; }
        public IList<EntityInfo> Entities { get; set; }
    }
}