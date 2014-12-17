namespace Ix.Palantir.DomainModel
{
    using System.Runtime.Serialization;

    using Ix.Palantir.Queueing.API.Command;

    public class CreateProjectCommand : CommandMessage
    {
        public override string CommandName
        {
            get
            {
                return "CreateProjectCommand";
            }
        }

        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int AccountId { get; set; }
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