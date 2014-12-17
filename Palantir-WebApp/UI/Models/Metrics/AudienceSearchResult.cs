namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AudienceSearchResult
    {
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "code")]
        public long FilterCode { get; set; }
    }
}