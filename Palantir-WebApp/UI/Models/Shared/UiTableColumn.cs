namespace Ix.Palantir.UI.Models.Shared
{
    using System.Collections.Generic;

    public class UiTableColumn
    {
        public UiTableColumn(string title)
        {
            this.Title = title;
            this.Items = new List<UiTableCellValue>();
        }

        public string Title { get; set; }
        public IList<UiTableCellValue> Items { get; private set; }

        public void AddItem(object item, string rowCssClass = "", bool tooltip = false)
        {
            UiTableCellValue value = new UiTableCellValue(item, rowCssClass, tooltip);
            this.Items.Add(value);
        }
        public void AddItem(object item, int rank)
        {
            UiTableCellValue value = new UiTableCellValue(item, rank);
            this.Items.Add(value);
        }
    }
}