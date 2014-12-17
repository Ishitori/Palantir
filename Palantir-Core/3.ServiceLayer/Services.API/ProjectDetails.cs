namespace Ix.Palantir.Services.API
{
    using System;

    public class ProjectDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string VkName { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastPostDate { get; set; }
    }
}