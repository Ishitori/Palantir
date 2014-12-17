namespace Ix.Palantir.Services.API.Export
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ExportExecutionResult
    {
        [DataMember(Name = "ticketId")]
        public string TicketId { get; set; }
        [DataMember(Name = "isFinished")]
        public bool IsFinished { get; set; }
        [DataMember(Name = "isSuccess")]
        public bool IsSuccess { get; set; }
        [DataMember(Name = "fileUrl")]
        public string FileUrl { get; set; }
    }
}