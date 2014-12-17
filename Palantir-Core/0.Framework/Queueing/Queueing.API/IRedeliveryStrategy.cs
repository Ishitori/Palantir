namespace Ix.Palantir.Queueing.API
{
    public interface IRedeliveryStrategy
    {
        void ProcessRedelivery(IMessage message);
    }
}