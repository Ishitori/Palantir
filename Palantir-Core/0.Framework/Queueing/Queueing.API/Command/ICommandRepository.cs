namespace Ix.Palantir.Queueing.API.Command
{
    using System;
    using System.Collections.Generic;

    public interface ICommandRepository
    {
        IList<string> GetSupportedCommandNames();
        Type GetCommandSystemType(string commandName);
        void RegisterCommand(ICommandMessage command);
    }
}
