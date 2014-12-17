namespace Ix.Palantir.Configuration
{
    using System.Xml.Serialization;

    [XmlRoot("feed-processing")]
    public class FeedProcessingConfig
    {
        [XmlAttribute("monthToFetch")]
        public int MonthToFetch { get; set; }
        [XmlAttribute("feedFilter")]
        public string FeedFilter { get; set; }
        [XmlAttribute("useGroupFilter")]
        public bool UseGroupFilter { get; set; }
        [XmlAttribute("groupQueueId")]
        public string GroupQueueId { get; set; }
        [XmlAttribute("inputQueueId")]
        public string InputQueueId { get; set; }
        [XmlAttribute("outputQueueId")]
        public string OutputQueueId { get; set; }
        [XmlAttribute("ttlInMinutes")]
        public int TtlInMinutes { get; set; }
    }
}