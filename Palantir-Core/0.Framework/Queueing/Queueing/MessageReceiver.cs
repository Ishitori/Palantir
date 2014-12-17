namespace Ix.Palantir.Queueing
{
    using System;
    using Ix.Palantir.Configuration.Queueing;
    using Ix.Palantir.Queueing.API;

    public class MessageReceiver : IMessageReceiver
    {
        private readonly IQueueingTransportLogger logger;
        private readonly Queue queue;
        private readonly ISessionProvider sessionProvider;
        
        private bool isDisposed;
        private Apache.NMS.IMessageConsumer messageConsumer;
        private Session session;

        public MessageReceiver(Queue queue, ISessionProvider sessionProvider, IQueueingTransportLogger logger)
        {
            this.queue = queue;
            this.sessionProvider = sessionProvider;
            this.logger = logger;
        }

        public string QueueName
        {
            get { return this.queue.Name; }
        }

        public string Selector { get; set; }

        public IMessage ReceiveMessage(int timeoutInMs = 1000)
        {
            this.InitializeIfReq();

            try
            {
                Apache.NMS.IMessage queueMessage = this.messageConsumer.Receive(TimeSpan.FromMilliseconds(timeoutInMs));
                this.LogMessageReceived(queueMessage);
                return Message.CreateFrom(queueMessage, this.session);
            }
            catch (Apache.NMS.NMSException exc)
            {
                throw new QueueingException("Cannot receive a message", exc);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void InitializeIfReq()
        {
            try
            {
                if (this.session == null || this.session.IsClosed)
                {
                    if (this.session != null)
                    {
                        this.session.Dispose();
                    }

                    if (this.messageConsumer != null)
                    {
                        this.messageConsumer.Dispose();
                    }

                    this.session = (Session)this.sessionProvider.GetSession(transactionalMode: false);
                    this.messageConsumer = this.session.CreateConsumer(this.queue, this.Selector);
                }
            }
            catch (Apache.NMS.NMSConnectionException exc)
            {
                throw new QueueingException("Cannot receive a message due to connection problems", exc);
            }
            catch (Apache.NMS.ActiveMQ.ConnectionClosedException exc)
            {
                throw new QueueingException("Cannot receive a message because connection is closed", exc);
            }
        }

        protected void LogMessageReceived(Apache.NMS.IMessage message)
        {
            Apache.NMS.ITextMessage textMessage = message as Apache.NMS.ITextMessage;

            if (textMessage != null)
            {
                this.logger.LogMessageWasReceived(textMessage.GetId(), textMessage.GetMessageType(), textMessage.GetTryIndex(), textMessage.GetSentDate(), textMessage.GetDeliveryDelay(), textMessage.Text);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.messageConsumer != null)
                {
                    this.messageConsumer.Dispose();
                }

                if (this.session != null)
                {
                    this.session.Dispose();
                }
            }

            this.messageConsumer = null;
            this.session = null;
            this.isDisposed = true;
        }
    }
}