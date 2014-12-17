namespace Ix.Palantir.Queueing
{
    using System;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Configuration.Queueing;
    using Ix.Palantir.Queueing.API;
    using IMessage = Ix.Palantir.Queueing.API.IMessage;

    public class Session : API.ISession
    {
        private readonly IConfigurationProvider configurationProvider;
        private readonly IConnectionNameProvider connectionNameProvider;
        private bool isDisposed;

        private Apache.NMS.IConnection queueConnection;
        private Apache.NMS.ISession queueSession;

        public Session(IConfigurationProvider configurationProvider, IConnectionNameProvider connectionNameProvider)
        {
            this.configurationProvider = configurationProvider;
            this.connectionNameProvider = connectionNameProvider;
        }

        public bool IsClosed
        {
            get { return !((Apache.NMS.ActiveMQ.Session)this.queueSession).Started; }
        }

        public bool IsTransactedMode
        {
            get { return this.queueSession.AcknowledgementMode == AcknowledgementMode.Transactional; }
        }

        public void Initialize(bool transactionMode)
        {
            Apache.NMS.IConnectionFactory connectionFactory = new Apache.NMS.ActiveMQ.ConnectionFactory(this.configurationProvider.GetConfigurationSection<QueueingConfig>().QueueServerUri);

            this.queueConnection = connectionFactory.CreateConnection();
            this.queueConnection.ClientId = this.connectionNameProvider.GetConnectionName();
            this.queueSession = this.queueConnection.CreateSession(transactionMode ? Apache.NMS.AcknowledgementMode.Transactional : Apache.NMS.AcknowledgementMode.AutoAcknowledge);
        }

        public void Commit()
        {
            try
            {
                if (this.IsTransactedMode)
                {
                    this.queueSession.Commit();
                }
            }
            catch (Apache.NMS.NMSException exc)
            {
                throw new QueueingException("Cannot commit session", exc);
            }
        }

        public void Rollback()
        {
            try
            {
                if (this.IsTransactedMode)
                {
                    this.queueSession.Rollback();
                }
            }
            catch (Apache.NMS.NMSException exc)
            {
                throw new QueueingException("Cannot rollback session", exc);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Apache.NMS.IMessageProducer CreateProducer(Queue queue)
        {
            if (this.queueConnection.IsStarted)
            {
                throw new QueueingException("Cannot create producer on an already opened connection. Create new session instead.");
            }

            this.OpenConnection(this.queueConnection, queue);
            Apache.NMS.IDestination destination = SessionUtil.GetDestination(this.queueSession, queue.Name);
            Apache.NMS.IMessageProducer producer = this.queueSession.CreateProducer(destination);
            producer.TimeToLive = queue.MessageTimeToLive;

            return producer;
        }

        public IMessageConsumer CreateConsumer(Queue queue, string selector = null)
        {
            if (this.queueConnection.IsStarted)
            {
                throw new QueueingException("Cannot create consumer on an already opened connection. Create new session instead.");
            }

            this.OpenConnection(this.queueConnection, queue);
            Apache.NMS.IDestination destination = SessionUtil.GetDestination(this.queueSession, queue.Name);
            Apache.NMS.IMessageConsumer consumer = this.queueSession.CreateConsumer(destination, selector);

            return consumer;
        }

        public Apache.NMS.ITextMessage CreateMessage(IMessage message)
        {
            Apache.NMS.ITextMessage textMessage = this.queueSession.CreateTextMessage(message.Text);
            textMessage.SetMessageType(message.Type);
            textMessage.SetTryIndex(message.TryIndex);
            textMessage.SetDeliveryDelay(message.DeliveryTimeout);
            textMessage.SetSentDate(DateTime.UtcNow);

            if (message.TtlInMinutes > 0)
            {
                textMessage.NMSTimeToLive = TimeSpan.FromMinutes(message.TtlInMinutes);
            }

            foreach (var property in message.ExtraProperties)
            {
                textMessage.Properties.SetString(property.Key, property.Value);
            }

            return textMessage;
        }

        public ITextMessage CreateMessage(IMessage message, TimeSpan messageTimeToLive)
        {
            ITextMessage textMessage = this.CreateMessage(message);

            if (messageTimeToLive != TimeSpan.Zero)
            {
                textMessage.NMSTimeToLive = messageTimeToLive;
            }

            return textMessage;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.queueSession != null)
                {
                    if (this.queueSession.AcknowledgementMode == AcknowledgementMode.Transactional)
                    {
                        this.queueSession.Rollback();
                    }

                    // there is a difference between calling dispose and Close on AMQ.Session object
                    this.queueSession.Close();
                }

                if (this.queueConnection != null)
                {
                    this.queueConnection.Dispose();
                }
            }

            this.queueSession = null;
            this.queueConnection = null;
            this.isDisposed = true;
        }

        private void OpenConnection(Apache.NMS.IConnection connection, Queue queue)
        {
            if (queue != null && !queue.RedeliveryPolicy.UseAMQScheduler)
            {
                connection.RedeliveryPolicy = new Apache.NMS.Policies.RedeliveryPolicy
                {
                    BackOffMultiplier = queue.RedeliveryPolicy.BackOffMultiplier,
                    InitialRedeliveryDelay = queue.RedeliveryPolicy.InitialRedeliveryDelay,
                    MaximumRedeliveries = queue.RedeliveryPolicy.MaximumRedeliveries,
                    UseExponentialBackOff = queue.RedeliveryPolicy.UseExponentialBackOff,
                    CollisionAvoidancePercent = queue.RedeliveryPolicy.CollisionAvoidancePercent,
                    UseCollisionAvoidance = queue.RedeliveryPolicy.UseCollisionAvoidance
                };
            }
            else
            {
                connection.RedeliveryPolicy = new Apache.NMS.Policies.RedeliveryPolicy
                {
                    MaximumRedeliveries = 0
                };
            }

            connection.Start();
        }
    }
}