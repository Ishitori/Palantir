namespace Ix.Palantir.DomainModel
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Queueing.API.Command;

    public class ExportReportCommand : CommandMessage
    {
        public override string CommandName
        {
            get { return "ExportReportCommand"; }
        }
        [DataMember]
        public virtual int VkGroupId { get; set; }
        [DataMember]
        public virtual string TicketId { get; set; }
        [DataMember]
        public virtual DateRange DateRange { get; set; }
        [DataMember]
        public int InitiatorUserId { get; set; }

        public override bool IsCorrupted()
        {
            return false;
        }
        public override void OnAfterDeserialization()
        {
        }
        public ExportReportCommand Copy()
        {
            ExportReportCommand itemCopy = new ExportReportCommand 
            {
                VkGroupId = this.VkGroupId,
                DateRange = this.DateRange,
                TicketId = this.TicketId,
                InitiatorUserId = this.InitiatorUserId
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