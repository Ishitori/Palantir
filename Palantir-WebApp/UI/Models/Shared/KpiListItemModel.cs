namespace Ix.Palantir.UI.Models.Shared
{
    using System.Web;
    using System.Web.Mvc;

    public class KpiListItemModel
    {
        private readonly string value;

        public KpiListItemModel(string id, string title, string value, ValueType valueType, string cssClass = "", bool encode = true)
        {
            this.Id = id;
            this.Title = title;
            this.value = value;
            this.ValueType = valueType;
            this.CssClass = cssClass;
            this.Encode = encode;
        }

        public string Title { get; set; }
        public string Id { get; set; }
        public MvcHtmlString Value
        {
            get
            {
                string data = this.Encode ? new HtmlString(this.value).ToHtmlString() : this.value;
                return new MvcHtmlString(data);
            }
        }
        public ValueType ValueType { get; set; }
        public bool Encode { get; set; }
        public string CssClass { get; set; }
    }
}