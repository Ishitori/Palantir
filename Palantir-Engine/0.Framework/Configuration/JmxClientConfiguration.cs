namespace Ix.Palantir.Engine.Configuration
{
    using System.Xml.Serialization;

    [XmlRoot("jmx-configuration")]
    public class JmxClientConfiguration
    {
        [XmlAttribute("amqJmxUrl")]
        public string AmqJmxUrl { get; set; }
        [XmlAttribute("brokerJmxName")]
        public string BrokerJmxName { get; set; }
    }
}