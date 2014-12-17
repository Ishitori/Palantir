namespace Ix.Palantir.UI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class GroupInfoRequest
    {
        public GroupInfoRequest()
        {
            this.StartDate = DateTime.Today.AddDays(-1);
            this.EndDate = DateTime.Today;
        }

        [Required(ErrorMessage = "����� �� �����")]
        public string VkLogin { get; set; }
        [Required(ErrorMessage = "������ �� �����")]
        public string VkPassword { get; set; }
        [Required(ErrorMessage = "����� ������ �� �����")]
        public string GroupUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public string GetGroupId()
        {
            if (string.IsNullOrWhiteSpace(this.GroupUrl))          
            {
                return string.Empty;    
            }

            var lastIndexOfSlash = this.GroupUrl.LastIndexOf('/');

            if (lastIndexOfSlash == -1)
            {
                return string.Empty;
            }

            return this.GroupUrl.Substring(lastIndexOfSlash + 1);
        }
    }
}