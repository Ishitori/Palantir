namespace Ix.Palantir.Queueing.API
{
    using Ix.Palantir.Configuration.Queueing;

    public interface IQueueingFactory
    {
        IMessageReceiver GetReceiver(Queue queue, string selector);
        IMessageSender GetSender(Queue queue);
    }
}