namespace Ix.Palantir.UI.Models.Chart
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ChartPoint
    {
        public ChartPoint(string x, double y)
        {
            this.X = x;
            this.Y = Math.Round(y, 3);
        }

        [DataMember(Name = "X")]
        public string X { get; set; }
        [DataMember(Name = "Y")]
        public double Y { get; set; }
    }
}