namespace Ix.Palantir.Queueing.API.Command
{
    using System;

    public interface ICommandSender : IDisposable
    {
        void SendCommand(ICommandMessage commandMessage);
        ICommandSender Open(string queueId);
    }
}
