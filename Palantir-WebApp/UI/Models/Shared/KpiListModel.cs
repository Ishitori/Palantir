namespace Ix.Palantir.UI.Models.Shared
{
    using System.Collections.Generic;

    public class KpiListModel
    {
        public KpiListModel(string title)
        {
            this.Title = title;
            this.Items = new List<KpiListItemModel>();
        }

        public string Title { get; set; }
        public IList<KpiListItemModel> Items { get; private set; }
        public string DetailsUrl { get; set; }
        public string CssClass { get; set; }

        public void AddItem(string title, double value, ValueType type, string cssClass = "")
        {
            this.Items.Add(new KpiListItemModel(title, title, value.ToString(), type, cssClass));
        }

        public void AddItem(string title, string value, ValueType type, string cssClass = "", bool encode = true)
        {
            this.Items.Add(new KpiListItemModel(title, title, value, type, cssClass, encode));
        }
    }
}