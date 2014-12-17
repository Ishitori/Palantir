namespace Ix.Palantir.Queueing.Command
{
    using System;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Configuration.Queueing;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Queueing.API;
    using Ix.Palantir.Queueing.API.Command;

    public class CommandReceiver : ICommandReceiver
    {
        private readonly IQueueingFactory queueingFactory;
        private readonly IConfigurationProvider configurationProvider;
        private readonly ICommandMessageMapper commandMapper;
        private readonly ICommandRepository commandRepository;
        
        private bool isDisposed;
        private IMessageReceiver receiver;

        public CommandReceiver(IQueueingFactory queueingFactory, IConfigurationProvider configurationProvider, ICommandMessageMapper commandMapper, ICommandRepository commandRepository)
        {
            this.queueingFactory = queueingFactory;
            this.configurationProvider = configurationProvider;
            this.commandMapper = commandMapper;
            this.commandRepository = commandRepository;
        }

        public ICommandReceiver Open(string queueId, string selector)
        {
            var queue = this.configurationProvider.GetConfigurationSection<QueueingConfig>().Queues.GetQueueById(queueId);
            this.receiver = this.queueingFactory.GetReceiver(queue, selector);

            return this;
        }
        public T GetCommand<T>() where T : ICommandMessage
        {
            var commandMessage = this.GetCommand();
            return (T)commandMessage;
        }

        public ICommandMessage GetCommand()
        {
            try
            {
                IMessage message = this.receiver.ReceiveMessage();

                if (message == null)
                {
                    return null;
                }

                return this.ConvertToCommand(message);
            }
            catch (Exception exc)
            {
                throw new QueueingException("Unable to receive command", exc);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private CommandMessage ConvertToCommand(IMessage message)
        {
            if (message == null)
            {
                return null;
            }

            if (message.IsUnsupported(this.commandRepository))
            {
                message.MarkAsFailedToProcess();
                return null;
            }

            try
            {
                var command = (CommandMessage)this.commandMapper.CreateCommand(message);

                if (command.IsCorrupted())
                {
                    message.MarkAsProcessed();
                    return null;
                }

                command.Message = message;
                return command;
            }
            catch (QueueingException exc)
            {
                throw new PalantirException(string.Format("Exception while creating command: {0}", message.Text), exc);
            }
        }
        private void Dispose(bool isDisposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                if (this.receiver != null)
                {
                    this.receiver.Dispose();
                    this.receiver = null;
                }
            }

            this.isDisposed = true;
        }
    }
}