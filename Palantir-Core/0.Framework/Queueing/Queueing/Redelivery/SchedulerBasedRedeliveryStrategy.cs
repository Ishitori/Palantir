namespace Ix.Palantir.Queueing.Redelivery
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Configuration.Queueing;
    using Ix.Palantir.Queueing.API;

    public class SchedulerBasedRedeliveryStrategy : IRedeliveryStrategy
    {
        private readonly Queue queue;
        private readonly IQueueingFactory queueingFactory;
        private readonly ISession session;

        public SchedulerBasedRedeliveryStrategy(ISession session, Queue queue)
        {
            this.queue = queue;
            this.session = session;
            this.queueingFactory = Factory.GetInstance<IQueueingFactory>();
        }

        public void ProcessRedelivery(IMessage message)
        {
            if (message.TryIndex == this.queue.RedeliveryPolicy.MaximumDeliveries && this.session.IsTransactedMode)
            {
                // Message couldn't be processed after all tries. That means we need to put it in DLQ
                // But DLQ processing works only with Transacted mode enabled, so if transacted mode is not used, we should change Queue address
                this.session.Rollback();
                return;
            }

            Queue queueToUse = this.queue;
            IMessage messageCopy = message.Copy();

            if (message.TryIndex == this.queue.RedeliveryPolicy.MaximumDeliveries)
            {
                queueToUse = this.queue.GetDLQ();
            }
            else
            {
                messageCopy.TryIndex += 1;
                messageCopy.DeliveryTimeout = this.CalculateDeliveryTimeout(messageCopy.TryIndex);
            }

            using (var messageSender = this.queueingFactory.GetSender(queueToUse))
            {
                messageSender.Send(messageCopy);
            }

            message.MarkAsProcessed();
        }

        private int CalculateDeliveryTimeout(int tryIndex)
        {
            if (tryIndex <= 0)
            {
                return 0;
            }

            RedeliveryPolicy policy = this.queue.RedeliveryPolicy;
            int retryIndex = tryIndex - 1;

            if (retryIndex == 0)
            {
                // this reproduces default ActiveMQ configuration when the first retry occurs immediately
                return 0;
            }

            if (!policy.UseExponentialBackOff)
            {
                return policy.InitialRedeliveryDelay;
            }

            int deliveryTimeout = Convert.ToInt32(policy.InitialRedeliveryDelay * Math.Pow(policy.BackOffMultiplier, retryIndex - 1)) - this.CalculateDeliveryTimeout(tryIndex - 1);

            return deliveryTimeout < 0
                       ? 0
                       : deliveryTimeout;
        }
    }
}