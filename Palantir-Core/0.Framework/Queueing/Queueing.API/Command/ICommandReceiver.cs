namespace Ix.Palantir.Queueing.API.Command
{
    using System;

    public interface ICommandReceiver : IDisposable
    {
        ICommandMessage GetCommand();
        ICommandReceiver Open(string queueId, string selector = null);
        T GetCommand<T>() where T : ICommandMessage;
    }
}
