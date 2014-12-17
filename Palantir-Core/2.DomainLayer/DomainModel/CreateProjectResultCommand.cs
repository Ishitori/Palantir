namespace Ix.Palantir.DomainModel
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Ix.Palantir.Queueing.API.Command;

    public class CreateProjectResultCommand : CommandMessage
    {
        public override string CommandName
        {
            get
            {
                return "CreateProjectResultCommand";
            }
        }

        [DataMember]
        public int VkGroupId { get; set; }
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public string TicketId { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public int ProjectId { get; set; }

        public override bool IsCorrupted()
        {
            return false;
        }

        public override void OnAfterDeserialization()
        {
        }

        public override IDictionary<string, string> GetProperties()
        {
            var properties = base.GetProperties();
            properties.Add("TicketId", this.TicketId);
            return properties;
        }
    }
}