namespace Ix.Palantir.DomainModel
{
    using System.Runtime.Serialization;
    using Ix.Palantir.Queueing.API.Command;

    public class UpdateMembersDeltaCommand : CommandMessage
    {
        public override string CommandName
        {
            get
            {
                return "UpdateMembersDeltaCommand";
            }
        }

        [DataMember]
        public virtual int VkGroupId { get; set; }
        [DataMember]
        public virtual int Version { get; set; }

        public override bool IsCorrupted()
        {
            return false;
        }

        public override void OnAfterDeserialization()
        {
        }
    }
}