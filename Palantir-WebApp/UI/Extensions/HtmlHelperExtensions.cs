namespace Ix.Palantir.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Ix.Palantir.UI.Models.Shared;
    using Ix.Palantir.UI.Renderers;

    public static class HtmlHelperExtensions
    {
        public static IHtmlString DatePicker(this HtmlHelper helper, string name)
        {
            return helper.DatePicker(name, null);
        }

        public static IHtmlString DatePicker(this HtmlHelper helper, string name, string imageUrl)
        {
            return helper.DatePicker(name, imageUrl, null);
        }

        public static IHtmlString DatePicker(this HtmlHelper helper, string name, object date)
        {
            return helper.DatePicker(name, "/Content/Images/calendar.gif", date);
        }

        public static IHtmlString DatePicker(this HtmlHelper helper, string name, string imageUrl, object boxedDate)
        {
            StringBuilder html = new StringBuilder();

            // Build our base input element
            html.Append("<input type=\"text\" id=\"" + name + "\" name=\"" + name + "\"");

            // Model Binding Support
            if (boxedDate != null)
            {
                string dateValue = string.Empty;
                DateTime date = (DateTime)boxedDate;

                if (date != DateTime.MinValue)
                {
                    dateValue = date.ToShortDateString();
                }

                html.Append(" value=\"" + dateValue + "\"");
            }

            // We're hard-coding the width here, a better option would be to pass in html attributes and reflect through them
            // here ( default to 75px width if no style attributes )
            html.Append(" style=\"width: 75px;\" />");

            // Now we call the datepicker function, passing in our options.  Again, a future enhancement would be to
            // pass in date options as a list of attributes ( min dates, day/month/year formats, etc. )
            html.Append("<script type=\"text/javascript\">$(document).ready(function() { $('#" + name + "').datepicker({ showOn: 'button', buttonImage: '" + imageUrl + "', duration: 0 }); });</script>");

            return new HtmlString(html.ToString());
        }

        public static MvcHtmlString Chart(this HtmlHelper helper, string id)
        {
            TagBuilder builder = new TagBuilder("div");
            builder.Attributes.Add("id", id);
            builder.Attributes.Add("class", "ui-chart");

            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString UiChart(this HtmlHelper helper, string id, string dataSourceUrl)
        {
            var legendContainer = new TagBuilder("div");
            legendContainer.AddCssClass("legendContainer");
            legendContainer.AddCssClass("clearfix");

            var container = new TagBuilder("div");
            container.Attributes.Add("id", id);
            container.Attributes.Add("data-source", dataSourceUrl);
            container.AddCssClass("chart-container");
            
            var chart = new TagBuilder("div");
            chart.AddCssClass("ui-chart");

            container.InnerHtml = legendContainer.ToString() + chart;
            return new MvcHtmlString(container.ToString());
        }

        public static MvcHtmlString UiPieChart(this HtmlHelper helper, string id, string dataSourceUrl)
        {
            var container = new TagBuilder("div");
            container.Attributes.Add("id", id);
            container.Attributes.Add("data-source", dataSourceUrl);
            container.AddCssClass("chart-container");
            
            var chart = new TagBuilder("div");
            chart.AddCssClass("ui-graph");

            container.InnerHtml = chart.ToString();

            return new MvcHtmlString(container.ToString());
        }

        public static MvcHtmlString UiTagCloud(this HtmlHelper helper, string id, string sourceUrl, int width, int height) 
        {
            var container = new TagBuilder("div");
            container.Attributes.Add("id", id);
            container.Attributes.Add("data-source", sourceUrl);
            container.Attributes.Add("style", string.Format("width:{0}%; height:{1}px;", width, height));
            container.AddCssClass("chart-container");
            return new MvcHtmlString(container.ToString());
        }

        public static MvcHtmlString UiTable(this HtmlHelper helper, IEnumerable<object> tableRows, string tableId)
        {
            var tableRenderer = new UiTableRenderer();
            return tableRenderer.RenderTable(tableRows, tableId);
        }
        public static MvcHtmlString UiColumnTable(this HtmlHelper helper, IList<UiTableColumn> tableColumns, string tableId)
        {
            var tableRenderer = new UiColumnTableRenderer();
            return tableRenderer.RenderTable(tableColumns, tableId);
        }
        
        public static MvcHtmlString CheckBoxList<T>(this HtmlHelper helper, string id, string name, string cssClass, IEnumerable<T> items, string textProperty, string valueProperty, IEnumerable<int> selectedItems)
        {
            Type itemstype = typeof(T);
            PropertyInfo textfieldInfo = itemstype.GetProperty(textProperty, typeof(string));
            PropertyInfo valuefieldInfo = itemstype.GetProperty(valueProperty);
            IList<string> selectedValues = selectedItems.Select(i => i.ToString()).ToList();

            var checklist = new StringBuilder();
            checklist.Append(string.Format("<ul id=\"{0}\" class=\"{1}\">", id, cssClass));

            foreach (var item in items)
            {
                var value = valuefieldInfo.GetValue(item, null).ToString();

                checklist.Append("<li>");

                var tag = new TagBuilder("input");
                tag.Attributes["type"] = "checkbox";
                tag.Attributes["value"] = value;
                tag.Attributes["name"] = name;
                tag.Attributes["id"] = name + value;

                if (selectedValues.Contains(value))
                {
                    tag.Attributes["checked"] = "checked";
                }
                
                checklist.Append(tag);
                checklist.Append(string.Format("<label for=\"{0}\">{1}</label>", name + value, textfieldInfo.GetValue(item, null)));
                checklist.Append("</li>");
            }

            checklist.Append("</ul>");
            return MvcHtmlString.Create(checklist.ToString());
        }
    }
}