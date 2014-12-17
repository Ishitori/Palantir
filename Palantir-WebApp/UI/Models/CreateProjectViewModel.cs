namespace Ix.Palantir.UI.Models
{
    using Ix.Palantir.Services.API;

    public class CreateProjectViewModel
    {
        public Project Project { get; set; }
        public bool CreateProjectsDisabled { get; set; }
    }
}