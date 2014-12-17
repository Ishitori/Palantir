namespace Ix.Palantir.Services.API
{
    using System.ComponentModel;

    public class Project
    {
        public int Id { get; set; }
        
        [DisplayName("Имя группы")]
        public string Title { get; set; }

        [DisplayName("URL группы")]
        public string Url { get; set; }
        public string VkName { get; set; }

        public int CreatorId { get; set; }
    }
}