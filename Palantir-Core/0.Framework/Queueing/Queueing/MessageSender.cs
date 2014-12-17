namespace Ix.Palantir.Queueing
{
    using System;
    using System.Threading;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Configuration.Queueing;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Queueing.API;

    public class MessageSender : IMessageSender
    {
        private readonly ILog log;
        private readonly Queue queue;
        private readonly IQueueingTransportLogger queueingTransportLogger;
        private readonly ISessionProvider sessionProvider;
        
        private bool isDisposed;
        private Apache.NMS.IMessageProducer messageProducer;
        private Session session;

        public MessageSender(Queue queue, ISessionProvider sessionProvider)
        {
            this.log = LogManager.GetLogger(ActivityType.Queueing.ToString());
            this.queue = queue;
            this.sessionProvider = sessionProvider;
            this.queueingTransportLogger = Factory.GetInstance<IQueueingTransportLogger>();
        }

        public void Send(IMessage message)
        {
            int tryIndex = 0;
            Exception exception;

            this.InitializeIfReq();
            Apache.NMS.ITextMessage textMessage = this.session.CreateMessage(message, this.queue.MessageTimeToLive);

            bool isSendingSuccessfull = this.DoSend(textMessage, out exception);

            if (isSendingSuccessfull)
            {
                return;
            }

            this.log.InfoFormat("Message couldn't be sent from the first try. Message.Id = {0}", textMessage.GetId());

            if (!this.queue.GuaranteeDelivery)
            {
                this.log.Info("Guaranteed delivery is disabled. Throwing an exception...");
                throw exception;
            }

            while (tryIndex < this.queue.GuaranteedDeliveryTriesCount)
            {
                tryIndex++;

                this.log.InfoFormat("Starting sending try index {0} for Message.Id = {1}...", tryIndex, textMessage.GetId());
                this.log.InfoFormat("Closing internal producers");
                this.CloseProducer();

                this.log.InfoFormat("Gaving the broker a chance to recover. Sleeping for 1 second");
                Thread.Sleep(1000);

                isSendingSuccessfull = this.DoSend(textMessage, out exception);

                if (isSendingSuccessfull)
                {
                    this.log.InfoFormat("Sending try index {0} for Message.Id = {1} succeeded.", tryIndex, textMessage.GetId());
                    return;
                }

                this.log.InfoFormat("Sending with try index {0} for Message.Id = {1} failed.", tryIndex, textMessage.GetId());
            }

            this.log.InfoFormat("Delivery try count limit of {0} tries is reached for Message.Id = {1}.", this.queue.GuaranteedDeliveryTriesCount, textMessage.GetId());

            if (exception != null)
            {
                this.log.InfoFormat("Throw exception for resending of Message.Id = {0}.", textMessage.GetId());
                throw exception;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.CloseProducer();
            }

            this.messageProducer = null;
            this.session = null;
            this.isDisposed = true;
        }
        
        private bool DoSend(Apache.NMS.ITextMessage textMessage, out Exception exception)
        {
            exception = null;

            try
            {
                this.InitializeIfReq();
                this.messageProducer.Send(textMessage);
                this.queueingTransportLogger.LogMessageWasSent(textMessage.GetId(), textMessage.GetMessageType(), textMessage.GetTryIndex(), textMessage.GetSentDate(), textMessage.GetDeliveryDelay(), textMessage.Text);
            }
            catch (Apache.NMS.NMSConnectionException exc)
            {
                exception = new QueueingException("Cannot send a message due to connection problems", exc);
            }
            catch (Apache.NMS.ActiveMQ.ConnectionClosedException exc)
            {
                exception = new QueueingException("Cannot send the message due to connection problem", exc);
            }
            catch (Apache.NMS.ActiveMQ.BrokerException exc)
            {
                exception = new QueueingException(string.Format("Broker exception happened while sending the message: {0}\r\n", textMessage.Text), exc);
            }
            catch (Apache.NMS.NMSException exc)
            {
                exception = new QueueingException(string.Format("Cannot send the message with text: {0}\r\n", textMessage.Text), exc);
            }

            return exception == null;
        }

        private void InitializeIfReq()
        {
            if (this.session == null || this.session.IsClosed)
            {
                if (this.session != null)
                {
                    this.session.Dispose();
                }

                if (this.messageProducer != null)
                {
                    this.messageProducer.Dispose();
                }

                this.session = (Session)this.sessionProvider.GetSession(transactionalMode: false);
                this.messageProducer = this.session.CreateProducer(this.queue);
            }
        }

        private void CloseProducer()
        {
            if (this.messageProducer != null)
            {
                this.messageProducer.Close();
                this.messageProducer.Dispose();
            }

            if (this.session != null)
            {
                this.session.Dispose();
            }

            this.messageProducer = null;
            this.session = null;
        }
    }
}