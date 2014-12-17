namespace Ix.Palantir.Security.API
{
    public interface IAccount
    {
        int Id { get; set; }
        string Title { get; set; }
        int? MaxProjectsCount { get; set; }
        bool CanDeleteProjects { get; set; }
    }
}