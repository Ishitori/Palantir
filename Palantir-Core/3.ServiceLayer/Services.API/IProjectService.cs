namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API.CreateProject;

    public interface IProjectService
    {
        IEnumerable<Project> GetProjects();
        IEnumerable<ProjectDetails> GetProjectsDetails();
        Project GetProject(int id);
        CreateProjectResult CreateProject(Project project);
        CreateProjectStatus GetCreateProjectStatus(string ticketId);
        IList<int> GetConcurentsIdsOf(int projectId);
        void DeleteProject(int projectId, int groupId);
    }
}