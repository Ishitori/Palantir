namespace Ix.Palantir.DomainModel
{
    using System.Runtime.Serialization;
    using Ix.Palantir.Queueing.API.Command;

    public class DeleteProjectCommand : CommandMessage
    {
        public override string CommandName
        {
            get
            {
                return "DeleteProjectCommand";
            }
        }

        [DataMember]
        public int ProjectId { get; set; }
        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        public string TicketId { get; set; }

        public override bool IsCorrupted()
        {
            return false;
        }

        public override void OnAfterDeserialization()
        {
        }
    }
}
