namespace Ix.Palantir.UI.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Ix.Palantir.UI.Models.Shared;

    public class UiColumnTableRenderer
    {
        private IList<UiTableColumn> data;
        private int rowsCount;
        private int columnsCount;

        public MvcHtmlString RenderTable(IList<UiTableColumn> tableColumns, string tableId)
        {
            Contract.Requires(tableColumns != null);
            this.RequiresSameNumberOfRows(tableColumns);

            if (!tableColumns.Any())
            {
                return new MvcHtmlString(string.Empty);
            }

            this.Initialize(tableColumns);

            var html = this.GetHtml(tableId);
            return new MvcHtmlString(html);
        }

        private string GetHtml(string tableId)
        {
            var table = new TagBuilder("table");
            table.Attributes.Add("id", tableId);
            table.AddCssClass("ui-column-table");
            table.AddCssClass("ui-table");
            table.AddCssClass("dataTable");

            table.InnerHtml += this.GenerateHead();
            table.InnerHtml += this.GenerateBody();

            return table.ToString();
        }

        private string GenerateHead()
        {
            var thead = new TagBuilder("thead");
            var theadTr = new TagBuilder("tr");
            var headerRow = this.data.Select(x => x.Items[0]).ToList();

            for (int i = 0; i < headerRow.Count; i++)
            {
                var item = headerRow[i];
                var th = new TagBuilder("th");

                if (i == 0)
                {
                    th.AddCssClass("firstColumn");
                }

                if (i % 2 == 0)
                {
                    th.AddCssClass("even-column");
                }

                th.Attributes.Add("id", "col" + i);

                th.InnerHtml = item.Value.ToString();
                theadTr.InnerHtml += th;
            }

            thead.InnerHtml += theadTr;
            return thead.ToString();
        }

        private string GenerateBody()
        {
            var body = new StringBuilder();

            for (int i = 1; i < this.rowsCount; i++)
            {
                var tr = new TagBuilder("tr");
                var row = this.data.Select(x => x.Items[i]).ToList();

                var th = new TagBuilder("th");
                th.Attributes.Add("id", "row" + i);
                if (this.data[0].Items[i].Tooltip)
                {
                    th.AddCssClass("tooltip");
                }
                th.AddCssClass("firstColumn");
                th.InnerHtml = row[0].Value.ToString();
                tr.InnerHtml += th;
                tr.AddCssClass(row[0].RowCssClass);

                for (int j = 1; j < this.columnsCount; j++)
                {
                    var item = row[j];
                    var td = new TagBuilder("td");

                    if (j % 2 == 0)
                    {
                        td.AddCssClass("even-column");
                    }

                    if (item.Rank != 0)
                    {
                        td.Attributes.Add("data-rank", item.Rank.ToString());
                    }

                    if (item.Value != null)
                    {
                        td.InnerHtml = item.Value.ToString();
                    }
                    else
                    {
                        td.AddCssClass("na-cell");
                        td.InnerHtml = "&nbsp;";
                    }

                    tr.InnerHtml += td;
                }

                body.Append(tr);
            }

            return body.ToString();
        }

        private void Initialize(IList<UiTableColumn> tableColumns)
        {
            this.data = tableColumns;
            this.rowsCount = this.data[0].Items.Count;
            this.columnsCount = this.data.Count;
        }
        private void RequiresSameNumberOfRows(IList<UiTableColumn> tableColumns)
        {
            int expectedNumberOfRows = tableColumns[0].Items.Count;

            for (int i = 1; i < tableColumns.Count; i++)
            {
                if (tableColumns[i].Items.Count != expectedNumberOfRows)
                {
                    throw new ArgumentException(string.Format("Number of items in \"{0}\" column is {1}. Expected {2}", i, tableColumns[i].Items.Count, expectedNumberOfRows), "tableColumns");
                }
            }
        }
    }
}