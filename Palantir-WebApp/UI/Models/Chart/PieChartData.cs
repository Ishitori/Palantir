namespace Ix.Palantir.UI.Models.Chart
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PieChartData
    {
        private static readonly IList<string> colors = new List<string> { "#4DA74D", "#CB4B4B", "#4C6F96", "#008FB2", "#9440ED", "#ACACAC", "#A23C3C", "#8CACC6", "#BD9B33" }; 

        public PieChartData()
        {
            this.Items = new List<PieChartDataItem>();
        }

        [DataMember(Name = "items")]
        public IList<PieChartDataItem> Items { get; private set; }

        public void AddItem(PieChartDataItem item)
        {
            item.Color = colors[this.Items.Count % colors.Count];
            this.Items.Add(item);
        }
    }
}