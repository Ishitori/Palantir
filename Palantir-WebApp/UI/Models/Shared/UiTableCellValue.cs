namespace Ix.Palantir.UI.Models.Shared
{
    public class UiTableCellValue
    {
        public UiTableCellValue(object item, string rowCssClass, bool tooltip = false)
        {
            this.Value = item;
            this.RowCssClass = rowCssClass;
            this.Tooltip = tooltip;
        }

        public UiTableCellValue(object item, int rank)
        {
            this.Value = item;
            this.Rank = rank;
        }

        public object Value { get; set; }
        public int Rank { get; set; }
        public string RowCssClass { get; set; }
        public bool Tooltip { get; set; }
    }
}