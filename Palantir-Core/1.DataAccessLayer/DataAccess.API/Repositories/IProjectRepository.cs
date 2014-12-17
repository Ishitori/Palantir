namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IProjectRepository
    {
        IEnumerable<Project> GetAll();
        ICollection<int> GetConcurrentIds(int accountId, int projectId);
        
        void Save(Project project);
        void Update(Project project);
        void Delete(Project project);

        VkGroup GetVkGroup(int projectId);
        IList<Project> GetByAccountId(int accountId);
        
        void DeleteAllConcurrents(int projectId);
        void AddConcurents(int projectId, IList<int> concurrentIds);
    }
}