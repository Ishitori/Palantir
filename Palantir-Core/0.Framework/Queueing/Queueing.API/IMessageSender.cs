namespace Ix.Palantir.Queueing.API
{
    using System;

    public interface IMessageSender : IDisposable
    {
        void Send(IMessage message);
    }
}
