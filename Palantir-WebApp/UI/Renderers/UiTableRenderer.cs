namespace Ix.Palantir.UI.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using Ix.Palantir.UI.Attributes;

    /// <summary>
    /// Класс для рендеринга Html элементов.
    /// </summary>
    public class UiTableRenderer
    {
        /// <summary>
        /// Создать таблицу.
        /// </summary>
        /// <param name="tableRows">Строки таблицы.</param>
        /// <param name="tableId">Id таблицы.</param>
        /// <returns>Таблица.</returns>
        public MvcHtmlString RenderTable(IEnumerable<object> tableRows, string tableId)
        {
            if (tableRows == null || !tableRows.Any())
            {
                return new MvcHtmlString(string.Empty);
            }

            var table = new TagBuilder("table");
            table.Attributes.Add("id", tableId);
            table.AddCssClass("ui-table");

            var columnType = tableRows.GetType().GetGenericArguments().FirstOrDefault();
            var columns = columnType.GetProperties().Where(p => p.GetCustomAttributes(typeof(UiTableColumnAttribute), false).Any());

            table.InnerHtml += this.GenerateThead(columns);
            table.InnerHtml += this.GenerateTbody(columns, tableRows);
            return new MvcHtmlString(table.ToString());
        }

        /// <summary>
        /// Заголовок таблицы.
        /// </summary>
        private TagBuilder GenerateThead(IEnumerable<PropertyInfo> columns)
        {
            var thead = new TagBuilder("thead");
            var theadTr = new TagBuilder("tr");
            var i = 0;

            foreach (var column in columns)
            {
                var attribute = column.GetCustomAttributes(typeof(UiTableColumnAttribute), false).FirstOrDefault() as UiTableColumnAttribute;
                var th = new TagBuilder("th");
                i++;

                if (i % 2 == 0)
                {
                    th.AddCssClass("even-column");
                }

                if (column.PropertyType == typeof(DateTime))
                {
                    th.AddCssClass("ui-table-datetime");
                }

                if (column.PropertyType == typeof(int) || column.PropertyType == typeof(double) || column.PropertyType == typeof(decimal))
                {
                    th.AddCssClass("ui-table-numeric");
                }

                if (attribute.AutoNumeric)
                {
                    th.AddCssClass("ui-table-autonumeric");
                }

                th.Attributes.Add("id", !string.IsNullOrWhiteSpace(attribute.ElementId) ? attribute.ElementId : "col" + i);
                switch (attribute.Sortable)
                {
                    case SortableBy.Asc: 
                        th.AddCssClass("ui-table-sortable-a"); 
                        break;

                    case SortableBy.Desc: 
                        th.AddCssClass("ui-table-sortable-d");
                        break;

                    case SortableBy.BothDirection: 
                        th.AddCssClass("ui-table-sortable"); 
                        break;
                }
                switch (attribute.DefaultSorted)
                {
                    case SortableBy.Asc: 
                        th.AddCssClass("ui-table-def-sorted-a"); 
                        break;

                    case SortableBy.Desc: 
                        th.AddCssClass("ui-table-def-sorted-d"); 
                        break;

                    case SortableBy.BothDirection: 
                        th.AddCssClass("ui-table-def-sorted"); 
                        break;
                }

                th.InnerHtml = attribute.DisplayName;
                theadTr.InnerHtml += th;
            }

            thead.InnerHtml += theadTr;
            return thead;
        }

        /// <summary>
        /// Тело таблицы.
        /// </summary>
        private TagBuilder GenerateTbody(IEnumerable<PropertyInfo> columns, IEnumerable<object> tableRows)
        {
            var tbody = new TagBuilder("tbody");
            var columnsCount = columns.Count();
            var i = 0;

            foreach (var row in tableRows)
            {
                var tr = new TagBuilder("tr");
                i++;

                if (i % 2 == 0)
                {
                    tr.AddCssClass("even-row");
                }

                var j = 0;

                foreach (var column in columns)
                {
                    var attribute = column.GetCustomAttributes(typeof(UiTableColumnAttribute), false).FirstOrDefault() as UiTableColumnAttribute;
                    var td = new TagBuilder("td");
                    
                    if (column.PropertyType == typeof(int) || column.PropertyType == typeof(double) || column.PropertyType == typeof(decimal))
                    {
                        td.AddCssClass("ui-td-numeric");
                    }

                    if (column.PropertyType == typeof(DateTime))
                    {
                        td.AddCssClass("ui-td-datetime");
                    }

                    if (attribute != null && attribute.AutoNumeric)
                    {
                        td.AddCssClass("ui-table-autonumeric");
                    }

                    if (j == columnsCount - 1)
                    {
                        td.AddCssClass("last-column");
                    }

                    j++;
                    
                    if (j % 2 == 0)
                    {
                        td.AddCssClass("even-column");
                    }

                    td.AddCssClass("col" + j);
                    td.InnerHtml = attribute.AutoNumeric 
                        ? i.ToString() 
                        : this.GetValue(column, row);
                    tr.InnerHtml += td;
                }

                if (i == 1)
                {
                    tr.AddCssClass("first");
                }

                if (i == tableRows.Count())
                {
                    tr.AddCssClass("last");
                }

                tbody.InnerHtml += tr;
            }
            return tbody;
        }

        private string GetValue(PropertyInfo column, object row)
        {
            var value = column.GetValue(row, new object[0]);

            if (column.PropertyType == typeof(DateTime))
            {
                var dateTimeValue = (DateTime)value;
                return string.Format("<span class=\"date\">{0}</span><span class=\"time\">{1}</span>", dateTimeValue.ToString("dd.MM.yyyy"), dateTimeValue.ToString("HH:mm"));
            }

            return value.ToString();
        }
    }
}