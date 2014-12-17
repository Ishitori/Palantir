namespace Ix.Palantir.Services.API.CreateProject
{
    using System.Runtime.Serialization;

    public class CreateProjectStatus
    {
        [DataMember(Name = "ticketId")]
        public string TicketId { get; set; }
        [DataMember(Name = "isFinished")]
        public bool IsFinished { get; set; }
        [DataMember(Name = "isSuccess")]
        public bool IsSuccess { get; set; }
        [DataMember(Name = "projectId")]
        public int ProjectId { get; set; }
        [DataMember(Name = "projectUrl")]
        public string ProjectUrl { get; set; }
    }
}