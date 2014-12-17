namespace Ix.Palantir.Queueing.Command
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.Queueing.API.Command;

    public class CommandRepository : ICommandRepository
    {
        private static readonly object lockObject = new object();
        private static readonly IList<ICommandMessage> commands;

        static CommandRepository()
        {
            commands = new List<ICommandMessage>();
        }
        public CommandRepository()
        {
        }
    
        public IList<string> GetSupportedCommandNames()
        {
            lock (lockObject)
            {
                return commands.Select(c => c.CommandName).ToList();
            }
        }
        public Type GetCommandSystemType(string commandName)
        {
            lock (lockObject)
            {
                return commands.Where(c => string.Compare(c.CommandName, commandName, true) == 0).Select(c => c.GetType()).FirstOrDefault();
            }
        }

        public void RegisterCommand(ICommandMessage command)
        {
            lock (lockObject)
            {
                commands.Add(command);
            }
        }
    }
}
