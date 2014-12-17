namespace Ix.Palantir.Services.API.Export
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ExportSchedulingResult
    {
        [DataMember(Name = "ticketId")]
        public string TicketId { get; set; }
    }
}