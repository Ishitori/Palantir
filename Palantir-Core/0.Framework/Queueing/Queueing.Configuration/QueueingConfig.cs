namespace Ix.Palantir.Queueing.Configuration
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("queueing")]
    public class QueueingConfig
    {
        [XmlArray("queues")]
        [XmlArrayItem("queue", typeof(Queue))]
        public List<Queue> Queues
        {
            get;
            set;
        }
        [XmlAttribute("enableTracing")]
        public bool EnableTracing
        {
            get;
            set;
        }
        [XmlAttribute("enableTransportLogging")]
        public bool EnableTransportLogging
        {
            get;
            set;
        }
        [XmlAttribute("transportLoggerBufferSize")]
        public int TransportLoggerBufferSize
        {
            get;
            set;
        }
        [XmlAttribute("transportLoggerMessageMaxLength")]
        public int TransportLoggerMessageMaxLength
        {
            get;
            set;
        }
        [XmlAttribute("serverConnectionString")]
        public string QueueServerUri
        {
            get;
            set;
        }
    }
}
