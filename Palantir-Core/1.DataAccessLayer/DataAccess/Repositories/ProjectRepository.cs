namespace Ix.Palantir.DataAccess.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class ProjectRepository : IProjectRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public ProjectRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public IEnumerable<Project> GetAll()
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Project>();
            }
        }

        public ICollection<int> GetConcurrentIds(int accountId, int projectId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                const string Query = "select pc.projectid2 from project p inner join projectconcurrent pc on (p.id = pc.projectid1) where accountid = @accountId and pc.projectid1 = @projectId";
                var concurrentIds = dataGateway.Connection.Query<int>(Query, new { accountId, projectId }).ToList();
                return concurrentIds;
            }
        }

        public IList<Project> GetByAccountId(int accountId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Project>().Where(p => p.AccountId == accountId).ToList();
            }
        }

        public void Save(Project project)
        {
            if (!project.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(project);
            }
        }
        public void Update(Project project)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.UpdateEntity(project);
            }
        }
        public void Delete(Project project)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                ////dataGateway.DeleteEntity(project); <- не работает!
                dataGateway.Connection.Execute("delete from project where id = @Id", new { Id = project.Id });
            }
        }

        public VkGroup GetVkGroup(int projectId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var project = dataGateway.GetEntities<Project>().SingleOrDefault(p => p.Id == projectId);
                return project != null ? project.VkGroup : null;
            }
        }

        public void DeleteAllConcurrents(int projectId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute("delete from projectconcurrent where projectid1 = @projectId", new { projectId });
            }
        }
        public void AddConcurents(int projectId, IList<int> concurrentIds)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                foreach (var concurrentId in concurrentIds)
                {
                    dataGateway.Connection.Execute("insert into projectconcurrent(projectid1, projectid2) values (@projectId1, @projectId2)", new { projectId1 = projectId, projectId2 = concurrentId });
                }
            }
        }
    }
}