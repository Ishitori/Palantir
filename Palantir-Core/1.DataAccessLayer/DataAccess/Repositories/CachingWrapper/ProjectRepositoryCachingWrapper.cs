namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System.Collections.Generic;
    using DomainModel;
    using Ix.Palantir.DataAccess.API.Repositories;

    public class ProjectRepositoryCachingWrapper : IProjectRepository
    {
        private readonly IProjectRepository internalRepository;
        private readonly IDictionary<int, VkGroup> groupsMap; 

        public ProjectRepositoryCachingWrapper(IProjectRepository internalRepository)
        {
            this.internalRepository = internalRepository;
            this.groupsMap = new Dictionary<int, VkGroup>();
        }

        public IEnumerable<Project> GetAll()
        {
            return this.internalRepository.GetAll();
        }
        public ICollection<int> GetConcurrentIds(int accountId, int projectId)
        {
            return this.internalRepository.GetConcurrentIds(accountId, projectId);
        }

        public IList<Project> GetByAccountId(int accountId)
        {
            return this.internalRepository.GetByAccountId(accountId);
        }

        public void Save(Project project)
        {
            this.internalRepository.Save(project);
        }
        public void Update(Project project)
        {
            this.internalRepository.Update(project);
        }
        public void Delete(Project project)
        {
            this.internalRepository.Delete(project);

            if (this.groupsMap.ContainsKey(project.Id))
            {
                this.groupsMap.Remove(project.Id);
            }
        }

        public void DeleteAllConcurrents(int projectId)
        {
            this.internalRepository.DeleteAllConcurrents(projectId);
        }
        public void AddConcurents(int projectId, IList<int> concurrentIds)
        {
            this.internalRepository.AddConcurents(projectId, concurrentIds);
        }
        public VkGroup GetVkGroup(int projectId)
        {
            if (!this.groupsMap.ContainsKey(projectId))
            {
                VkGroup vkGroup = this.internalRepository.GetVkGroup(projectId);
                this.groupsMap.Add(projectId, vkGroup);
            }

            return this.groupsMap[projectId];
        }
    }
}