namespace Ix.Palantir.Queueing.API.Command
{
    public interface ICommandProcessor
    {
        void Execute(ICommandMessage command);
    }
}
