namespace Ix.Palantir.StatServer.Queueing.Commands
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using Framework.ObjectFactory;

    using Ix.Palantir.Queueing.API;
    using Ix.Palantir.Queueing.API.Command;

    public class CommandMessageMapper : ICommandMessageMapper
    {
        private readonly ICommandRepository commandRepository;
        private readonly XmlWriterSettings xmlWriterSettings;

        public CommandMessageMapper(ICommandRepository commandRepository)
        {
            this.commandRepository = commandRepository;
            this.xmlWriterSettings = new XmlWriterSettings
                                         {
                                             Indent = true, 
                                             IndentChars = "\t", 
                                             NewLineChars = "\r\n", 
                                             NewLineHandling = NewLineHandling.Replace,
                                             CloseOutput = true,
                                             OmitXmlDeclaration = true,
                                             NewLineOnAttributes = false
                                         };
        }

        public ICommandMessage CreateCommand(IMessage message)
        {
            if (message == null)
            {
                return null;
            }

            Type commandSystemType = this.commandRepository.GetCommandSystemType(message.Type);
            DataContractSerializer serializer = new DataContractSerializer(commandSystemType);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(message.Text)))
            {
                ICommandMessage commandMessage = (ICommandMessage)serializer.ReadObject(stream);
                commandMessage.SendingDate = message.SentDate;
                commandMessage.TryIndex = message.TryIndex;
                commandMessage.OnAfterDeserialization();
                return commandMessage;
            }
        }
        public IMessage CreateMessage(ICommandMessage command)
        {
            if (command == null)
            {
                return null;
            }

            IMessage message = Factory.GetInstance<IMessage>();
            DataContractSerializer serializer = new DataContractSerializer(command.GetType());
            StringBuilder output = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(output, this.xmlWriterSettings))
            {
                serializer.WriteObject(writer, command);
                writer.Flush();
                message.Text = output.ToString();
                message.ExtraProperties = command.GetProperties();
                message.TtlInMinutes = command.TtlInMinutes;
            }

            /*using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, command);
                memoryStream.Position = 0;

                using (StreamReader reader = new StreamReader(memoryStream))
                {
                    message.Text = reader.ReadToEnd();
                }
            }*/ 
            
            message.Type = command.CommandName;
            message.SentDate = command.SendingDate;
            return message;
        }
    }
}
