namespace Ix.Palantir.DomainModel
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Queueing.API.Command;

    public class ExportResultCommand : CommandMessage
    {
        public override string CommandName
        {
            get { return "ExportResultCommand"; }
        }
        [DataMember]
        public virtual int VkGroupId { get; set; }
        [DataMember]
        public virtual string TicketId { get; set; }
        [DataMember]
        public virtual DateRange DateRange { get; set; }
        [DataMember]
        public int InitiatorUserId { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }

        public override bool IsCorrupted()
        {
            return false;
        }
        public override void OnAfterDeserialization()
        {
        }
        public ExportResultCommand Copy()
        {
            ExportResultCommand itemCopy = new ExportResultCommand
            {
                VkGroupId = this.VkGroupId,
                DateRange = this.DateRange,
                TicketId = this.TicketId,
                InitiatorUserId = this.InitiatorUserId,
                FilePath = this.FilePath
            };

            return itemCopy;
        }

        public override IDictionary<string, string> GetProperties()
        {
            var properties = base.GetProperties();
            properties.Add("TicketId", this.TicketId);
            return properties;
        }
    }
}