namespace Ix.Palantir.UI.Models.Shared
{
    public class KpiValueModel
    {
        public KpiValueModel() : this(string.Empty, 0, string.Empty)
        {
        }
        public KpiValueModel(string title, double value) : this(title, value, string.Empty)
        {
        }
        public KpiValueModel(string title, double value, string detailsUrl)
        {
            this.Title = title;
            this.Value = value;
            this.DetailsUrl = detailsUrl;
        }

        public string Title { get; set; }
        public double Value { get; set; }
        public string DetailsUrl { get; set; }
    }
}