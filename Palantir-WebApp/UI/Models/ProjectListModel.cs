namespace Ix.Palantir.UI.Models
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API;

    public class ProjectListModel
    {
        public ProjectListModel(IEnumerable<ProjectDetails> details)
        {
            this.Projects = new List<ProjectListModelItem>();

            foreach (var detail in details)
            {
                this.Projects.Add(new ProjectListModelItem(detail));
            }
        }

        public IList<ProjectListModelItem> Projects { get; private set; }
    }
}