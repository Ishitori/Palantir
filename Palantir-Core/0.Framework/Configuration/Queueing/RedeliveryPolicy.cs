namespace Ix.Palantir.Configuration.Queueing
{
    using System.Xml.Serialization;

    public class RedeliveryPolicy
    {
        public int MaximumDeliveries
        {
            get
            {
                return this.MaximumRedeliveries;
            }
        }
        [XmlAttribute("maximumRedeliveries")]
        public int MaximumRedeliveries
        {
            get; 
            set;
        }
        [XmlAttribute("initialRedeliveryDelay")]
        public int InitialRedeliveryDelay
        {
            get;
            set;
        }
        [XmlAttribute("useExponentialBackOff")]
        public bool UseExponentialBackOff
        {
            get;
            set;
        }
        [XmlAttribute("backOffMultiplier")]
        public int BackOffMultiplier
        {
            get;
            set;
        }
        [XmlAttribute("collisionAvoidancePercent")]
        public int CollisionAvoidancePercent
        {
            get; 
            set;
        }
        [XmlAttribute("useCollisionAvoidance")]
        public bool UseCollisionAvoidance
        {
            get;
            set;
        }
        [XmlAttribute("useAMQScheduler")]
        public bool UseAMQScheduler
        {
            get;
            set;
        }
    }
}
