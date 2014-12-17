namespace Ix.Palantir.UI.Models.Chart
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ChartPoint_WithPercents : ChartPoint
    {
        public ChartPoint_WithPercents(string x, double y, string perc_allUsers, string perc_activeUsers) : base(x, y)
        {
            this.Perc_allUsers = perc_allUsers;
            this.Perc_activeUsers = perc_activeUsers;
        }

        [DataMember(Name = "perc_allUsers")]
        public string Perc_allUsers { get; set; }

        [DataMember(Name = "perc_activeUsers")]
        public string Perc_activeUsers { get; set; }
    }
}