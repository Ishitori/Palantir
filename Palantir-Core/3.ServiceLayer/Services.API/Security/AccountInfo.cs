namespace Ix.Palantir.Services.API.Security
{
    using System.ComponentModel;

    using Ix.Palantir.Security.API;

    public class AccountInfo
    {
        public AccountInfo()
        {
        }
        public AccountInfo(IAccount account)
        {
            this.Id = account.Id;
            this.Title = account.Title;
            this.MaxProjectsCount = account.MaxProjectsCount;
            this.CanDeleteProjects = account.CanDeleteProjects;
        }

        public int Id { get; set; }

        [DisplayName("Наименование")]
        public string Title { get; set; }

        [DisplayName("Макс. кол-во проектов")]
        public int? MaxProjectsCount { get; set; }

        [DisplayName("Может удалять проекты")]
        public bool CanDeleteProjects { get; set; }
    }
}
