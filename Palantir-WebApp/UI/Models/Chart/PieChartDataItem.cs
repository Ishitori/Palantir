namespace Ix.Palantir.UI.Models.Chart
{
    using System.Runtime.Serialization;

    [DataContract]
    public class PieChartDataItem
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }
        [DataMember(Name = "data")]
        public int Value { get; set; }
        [DataMember(Name = "color")]
        public string Color { get; set; }
    }
}