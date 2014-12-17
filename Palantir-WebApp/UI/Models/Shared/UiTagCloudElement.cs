namespace Ix.Palantir.UI.Models.Shared
{
    /// <summary>
    /// Представление элемента облака тегов.
    /// </summary>
    public class UiTagCloudElement
    {
        public string DataId { get; set; }

        public string Text { get; set; }
        public int Weight { get; set; }

        public string Href { get; set; }
    }
}