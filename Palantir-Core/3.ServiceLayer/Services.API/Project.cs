namespace Ix.Palantir.Services.API
{
    using System.ComponentModel;

    public class Project
    {
        public int Id { get; set; }
        
        [DisplayName("��� ������")]
        public string Title { get; set; }

        [DisplayName("URL ������")]
        public string Url { get; set; }
        public string VkName { get; set; }

        public int CreatorId { get; set; }
    }
}