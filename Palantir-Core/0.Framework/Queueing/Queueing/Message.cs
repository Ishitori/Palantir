namespace Ix.Palantir.Queueing
{
    using System;
    using System.Collections.Generic;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Queueing.API;

    public class Message : IMessage
    {
        private readonly IRedeliveryStrategyFactory redeliveryFactory;

        private Apache.NMS.IMessage queueMessage;
        private ISession session;

        public Message()
        {
            this.TryIndex = 1;
            this.redeliveryFactory = Factory.GetInstance<IRedeliveryStrategyFactory>();
            this.ExtraProperties = new Dictionary<string, string>();
        }

        public string Id { get; private set; }
        public string Type { get; set; }
        public string Text { get; set; }

        public IDictionary<string, string> ExtraProperties { get; set; }
        public int TryIndex { get; set; }
        public DateTime SentDate { get; set; }

        public string QueueName
        {
            get { return ((Apache.NMS.ActiveMQ.Commands.ActiveMQDestination)this.queueMessage.NMSDestination).PhysicalName; }
        }

        public int DeliveryTimeout { get; set; }

        public int TtlInMinutes { get; set; }

        public static IMessage CreateFrom(Apache.NMS.IMessage queueMessage, ISession session)
        {
            if (queueMessage == null)
            {
                return null;
            }

            Apache.NMS.ITextMessage textMessage = queueMessage as Apache.NMS.ITextMessage;

            if (textMessage == null)
            {
                throw new ArgumentException(string.Format("Unsupported message type provided: \"{0}\"", queueMessage.GetType().FullName));
            }

            Message message = new Message();
            message.queueMessage = queueMessage;
            message.Text = textMessage.Text;
            message.Id = textMessage.GetId();
            message.Type = textMessage.GetMessageType();
            message.SentDate = textMessage.GetSentDate();
            message.TryIndex = textMessage.GetTryIndex();
            message.DeliveryTimeout = textMessage.GetDeliveryDelay();
            message.session = session;

            return message;
        }

        public IMessage Copy()
        {
            IMessage messageCopy = new Message
            {
                Type = this.Type,
                TryIndex = this.TryIndex,
                Text = this.Text,
                TtlInMinutes = this.TtlInMinutes
            };

            return messageCopy;
        }

        public void MarkAsProcessed()
        {
            if (this.queueMessage == null)
            {
                return;
            }

            this.queueMessage.Acknowledge();

            if (this.session != null)
            {
                this.session.Commit();
            }
        }

        public void MarkAsFailedToProcess()
        {
            var redeliveryStrategy = this.redeliveryFactory.CreateStrategy(this.QueueName, this.session);
            redeliveryStrategy.ProcessRedelivery(this);
        }
    }
}