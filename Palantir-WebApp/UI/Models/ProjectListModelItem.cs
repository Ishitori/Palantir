namespace Ix.Palantir.UI.Models
{
    using Ix.Palantir.Services.API;

    public class ProjectListModelItem
    {
        private readonly ProjectDetails details;

        public ProjectListModelItem(ProjectDetails details)
        {
             this.details = details;
        }

        public int Id
        {
            get { return this.details.Id; }
        }
        public string Title
        {
            get { return this.details.Title; }
        }
        public string Url
        {
            get { return this.details.Url; }
        }
        public string CreationDate
        {
            get 
            {
                return this.details.CreationDate.HasValue
                    ? this.details.CreationDate.Value.ToString("dd.MM.yyyy")
                    : "-";
            }
        }
        public string CreationTime
        {
            get 
            {
                return this.details.CreationDate.HasValue
                    ? this.details.CreationDate.Value.ToString("HH:mm")
                    : string.Empty;
            }
        }
        public string LastPostDate
        {
            get
            {
                return this.details.LastPostDate.HasValue
                    ? this.details.LastPostDate.Value.ToString("dd.MM.yyyy")
                    : "-";
            }
        }
        public string LastPostTime
        {
            get 
            {
                return this.details.LastPostDate.HasValue
                    ? this.details.LastPostDate.Value.ToString("HH:mm")
                    : string.Empty;
            }
        }
    }
}