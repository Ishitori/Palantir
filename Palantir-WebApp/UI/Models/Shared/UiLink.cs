namespace Ix.Palantir.UI.Models.Shared
{
    using System.Web.Mvc;

    /// <summary>
    /// Данные для отображения Html ссылки.
    /// </summary>
    public class UiLink
    {
        /// <summary>
        /// Адрес ссылки.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Текст.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Цель.
        /// </summary>
        public string Target { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Url))
            {
                return this.Text;
            }

            var a = new TagBuilder("a");
            a.Attributes.Add("href", this.Url);

            if (!string.IsNullOrWhiteSpace(this.Target))
            {
                a.Attributes.Add("target", this.Target);
            }

            a.InnerHtml = this.Text;
            return a.ToString();
        }
    }
}