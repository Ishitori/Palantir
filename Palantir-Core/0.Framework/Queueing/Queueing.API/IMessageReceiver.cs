namespace Ix.Palantir.Queueing.API
{
    using System;

    public interface IMessageReceiver : IDisposable
    {
        string QueueName { get; }
        string Selector { get; set; }

        IMessage ReceiveMessage(int timeoutInMs = 1000);
    }
}
