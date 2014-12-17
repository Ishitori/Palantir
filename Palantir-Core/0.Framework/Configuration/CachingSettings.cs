namespace Ix.Palantir.Configuration
{
    using System.Xml.Serialization;

    [XmlRoot("interface")]
    public class CachingSettings
    {
        public CachingSettings()
        {
            this.TTLInMinutes = 5;
            this.CachingMode = CachingMode.Default;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

        [XmlAttribute("ttlInMinutes")]
        public int TTLInMinutes { get; set; }
        
        [XmlAttribute("cachingMode")]
        public CachingMode CachingMode { get; set; }
    }
}