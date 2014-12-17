namespace Ix.Palantir.Queueing.Command
{
    using System;
    using System.Diagnostics.Contracts;
    using Configuration.Queueing;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Queueing.API;
    using Ix.Palantir.Queueing.API.Command;

    public class CommandSender : ICommandSender
    {
        private readonly IQueueingFactory queueingFactory;
        private readonly ICommandMessageMapper commandMapper;
        private readonly IConfigurationProvider configurationProvider;

        private bool isDisposed;
        private IMessageSender messageSender;

        public CommandSender(IQueueingFactory queueingFactory, ICommandMessageMapper commandMapper, IConfigurationProvider configurationProvider)
        {
            this.queueingFactory = queueingFactory;
            this.commandMapper = commandMapper;
            this.configurationProvider = configurationProvider;
        }

        public ICommandSender Open(string queueId)
        {
            var queue = this.configurationProvider.GetConfigurationSection<QueueingConfig>().Queues.GetQueueById(queueId);
            this.messageSender = this.queueingFactory.GetSender(queue);
            return this;
        }

        public void SendCommand(ICommandMessage commandMessage)
        {
            Contract.Requires(commandMessage != null, "Command message is null. Nothing to send");

            if (this.messageSender == null)
            {
                throw new QueueingException("MessageSender is null. Call Open(string queueId) before tryingto send a command");
            }

            IMessage message = this.commandMapper.CreateMessage(commandMessage);
            this.messageSender.Send(message);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.messageSender != null)
            {
                this.messageSender.Dispose();
                this.messageSender = null;
            }

            this.isDisposed = true;
        }
    }
}
