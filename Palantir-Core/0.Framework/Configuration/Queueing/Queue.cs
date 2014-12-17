namespace Ix.Palantir.Configuration.Queueing
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot("queue")]
    public class Queue
    {
        private const string CONST_DLQSuffix = "DLQ.";
        private const string CONST_QueuePrefix = "queue://";

        private string queueName;

        public Queue()
        {
            this.MessageTimeToLive = TimeSpan.Zero;
        }

        [XmlAttribute("id")]
        public string Id
        {
            get;
            set;
        }
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(this.queueName);
            }
            set
            {
                this.queueName = value;
            }
        }
        [XmlAttribute("enableSendersPooling")]
        public bool EnableSendersPooling
        {
            get;
            set;
        }
        [XmlAttribute("maxPoolSize")]
        public int MaxPoolSize
        {
            get;
            set;
        }
        [XmlAttribute("idleTimeout")]
        public int IdleTimeout
        {
            get;
            set;
        }
        [XmlElement("redeliveryPolicy")]
        public RedeliveryPolicy RedeliveryPolicy
        {
            get; 
            set;
        }
        [XmlAttribute("dlqTtl")]
        public int DLQTimeToLive
        {
            get;
            set;
        }
        public TimeSpan MessageTimeToLive { get; set; }
        [XmlAttribute("guaranteeDelivery")]
        public bool GuaranteeDelivery
        {
            get; 
            set;
        }
        [XmlAttribute("guaranteedDeliveryTriesCount")]
        public int GuaranteedDeliveryTriesCount
        {
            get;
            set;
        }

        public Queue GetDLQ()
        {
            Queue dlq = new Queue()
            {
                EnableSendersPooling = false,
                Id = string.Format("{0}{1}", CONST_DLQSuffix, this.Id),
                IdleTimeout = this.IdleTimeout,
                MaxPoolSize = 0,
                Name = this.GetDlqQueueName(),
                RedeliveryPolicy = this.RedeliveryPolicy,
                GuaranteeDelivery = this.GuaranteeDelivery,
                GuaranteedDeliveryTriesCount = this.GuaranteedDeliveryTriesCount,
                DLQTimeToLive = this.DLQTimeToLive,
                MessageTimeToLive = this.DLQTimeToLive == 0 ? TimeSpan.Zero : TimeSpan.FromMinutes(this.DLQTimeToLive)
            };

            return dlq;
        }

        private string GetDlqQueueName()
        {
            if (string.IsNullOrWhiteSpace(this.queueName))
            {
                return string.Empty;
            }

            int queuePrefixIndex = this.queueName.IndexOf(CONST_QueuePrefix);

            if (queuePrefixIndex == -1)
            {
                // invalid format of queue
                return string.Empty;
            }

            return this.queueName.Insert(queuePrefixIndex + CONST_QueuePrefix.Length, CONST_DLQSuffix);
        }
    }
}
