namespace Ix.Palantir.Queueing.API.Command
{
    public interface ICommandMessageMapper
    {
        ICommandMessage CreateCommand(IMessage message);
        IMessage CreateMessage(ICommandMessage command);
    }
}
