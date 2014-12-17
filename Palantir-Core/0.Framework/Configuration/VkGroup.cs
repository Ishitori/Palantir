namespace Ix.Palantir.Configuration
{
    using System.Xml.Serialization;

    [XmlRoot("group")]
    public class VKGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
    }
}