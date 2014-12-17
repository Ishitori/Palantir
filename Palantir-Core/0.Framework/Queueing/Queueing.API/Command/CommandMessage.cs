namespace Ix.Palantir.Queueing.API.Command
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class CommandMessage : ICommandMessage
    {
        protected CommandMessage()
        {
            this.SendingDate = DateTime.UtcNow;
        }

        public abstract string CommandName
        {
            get;
        }
        public DateTime SendingDate
        {
            get;
            set;
        }
        public int TryIndex
        {
            get;
            set;
        }
        public int TtlInMinutes { get; set; }

        public IMessage Message { get; set; }

        public abstract bool IsCorrupted();
        public abstract void OnAfterDeserialization();
        public virtual IDictionary<string, string> GetProperties()
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            return properties;
        }

        public void MarkAsCompleted()
        {
            if (this.Message != null)
            {
                this.Message.MarkAsProcessed();
            }
        }
    }
}
