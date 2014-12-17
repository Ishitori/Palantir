namespace Ix.Palantir.Queueing.Logging
{
    using System;

    public class QueueingLogItem
    {
        private readonly QueueingLogActionType type;
        private readonly string messageId;
        private readonly string messageType;
        private readonly int messageTryIndex;
        private readonly DateTime messageSentDate;
        private readonly string messageText;
        private readonly int deliveryDelay;
        private readonly DateTime loggingDate;

        public QueueingLogItem(QueueingLogActionType type, string messageId, string messageType, int messageTryIndex, DateTime messageSentDate, int deliveryDelay, string messageText)
        {
            this.type = type;
            this.messageId = messageId;
            this.messageType = messageType;
            this.messageTryIndex = messageTryIndex;
            this.messageSentDate = messageSentDate;
            this.messageText = messageText;
            this.messageId = messageId;
            this.deliveryDelay = deliveryDelay;
            this.loggingDate = DateTime.UtcNow;
        }

        public QueueingLogActionType Type
        {
            get
            {
                return this.type;
            }
        }
        public string MessageId
        {
            get
            {
                return this.messageId;
            }
        }
        public string MessageType
        {
            get
            {
                return this.messageType;
            }
        }
        public int MessageTryIndex
        {
            get
            {
                return this.messageTryIndex;
            }
        }
        public DateTime MessageSentDate
        {
            get
            {
                return this.messageSentDate;
            }
        }
        public int DeliveryDelay
        {
            get
            {
                return this.deliveryDelay;
            }
        }
        public string MessageText
        {
            get
            {
                return this.messageText;
            }
        }
        public DateTime LoggingDate
        {
            get
            {
                return this.loggingDate;
            }
        }
    }
}