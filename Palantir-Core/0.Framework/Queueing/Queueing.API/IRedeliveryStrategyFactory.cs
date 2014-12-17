namespace Ix.Palantir.Queueing.API
{
    public interface IRedeliveryStrategyFactory
    {
        IRedeliveryStrategy CreateStrategy(string queueName, ISession session);
    }
}