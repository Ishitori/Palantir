namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Ix.Palantir.UI.Models.Chart;

    [DataContract]
    public class ActiveUserJsonModel
    {
        [DataMember]
        public string Table { get; set; }

        [DataMember]
        public string InterestsData { get; set; }

        [DataMember]
        public PieChartData AgeData { get; set; }

        [DataMember]
        public PieChartData GenderData { get; set; }

        [DataMember]
        public PieChartData EducationData { get; set; }

        [DataMember]
        public string CountryAndCityData { get; set; }
    }
}