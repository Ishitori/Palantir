namespace Ix.Palantir.Queueing.Redelivery
{
    using Ix.Palantir.Queueing.API;

    public class PolicyBasedRedeliveryStrategy : IRedeliveryStrategy
    {
        private readonly ISession session;

        public PolicyBasedRedeliveryStrategy(ISession session)
        {
            this.session = session;
        }

        public void ProcessRedelivery(IMessage message)
        {
            if (this.session == null)
            {
                return;
            }

            this.session.Rollback();
        }
    }
}