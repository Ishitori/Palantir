namespace Ix.Palantir.Queueing.Redelivery
{
    using Configuration.Queueing;

    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Queueing.API;

    public class RedeliveryStrategyFactory : IRedeliveryStrategyFactory
    {
        private readonly IConfigurationProvider configurationProvider;

        public RedeliveryStrategyFactory(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IRedeliveryStrategy CreateStrategy(string queueName, ISession session)
        {
            var queue = this.configurationProvider.GetConfigurationSection<QueueingConfig>().Queues.GetQueueByName(queueName);
            return queue.RedeliveryPolicy.UseAMQScheduler
                        ? (IRedeliveryStrategy)new SchedulerBasedRedeliveryStrategy(session, queue)
                        : new PolicyBasedRedeliveryStrategy(session);
        }
    }
}