namespace Ix.Palantir.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using DomainModel;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.CreateProject;
    using Security;
    using Security.API;

    using Project = Ix.Palantir.Services.API.Project;

    public class ProjectService : IProjectService
    {
        private const string CONST_CreateProjectQueueName = "CreateProjectQueue";
        private const string CONST_CreateProjectResultQueueName = "CreateProjectResultQueue";
        private const string CONST_DeleteProjectQueueName = "DeleteProjectQueue";
        
        private readonly IProjectRepository projectRepository;
        private readonly IStatisticsProvider statisticsProvider;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IUnitOfWorkProvider unitOfWorkProvider;

        public ProjectService(IProjectRepository projectRepository, IStatisticsProvider statisticsProvider, IDateTimeHelper dateTimeHelper, ICurrentUserProvider currentUserProvider, IUnitOfWorkProvider unitOfWorkProvider)
        {
            this.projectRepository = projectRepository;
            this.statisticsProvider = statisticsProvider;
            this.dateTimeHelper = dateTimeHelper;
            this.currentUserProvider = currentUserProvider;
            this.unitOfWorkProvider = unitOfWorkProvider;
        }

        public IEnumerable<Project> GetProjects()
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                int accountId = this.currentUserProvider.GetCurrentUser().GetAccount().Id;
                var projects = this.projectRepository.GetByAccountId(accountId).Select(e => new Project
                {
                    Id = e.Id,
                    Title = e.Title,
                    Url = e.VkGroup.Url,
                    VkName = e.VkGroup.Name,
                    CreatorId = e.AccountId
                }).ToList();

                return projects;
            }
        }
        public IEnumerable<ProjectDetails> GetProjectsDetails()
        {
            IList<ProjectDetails> details = new List<ProjectDetails>();

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var accountId = this.currentUserProvider.GetCurrentUser().GetAccount().Id;
                IList<DomainModel.Project> projects = this.projectRepository.GetByAccountId(accountId);

                foreach (var project in projects)
                {
                    var firstPostDate = this.statisticsProvider.GetFirstPostDate(project.Id);
                    var lastPostDate = this.statisticsProvider.GetLastPostDate(project.Id);

                    var projectDetails = new ProjectDetails
                    {
                        Id = project.Id,
                        Title = project.Title,
                        Url = project.VkGroup.Url,
                        CreationDate = firstPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(firstPostDate.Value) : (DateTime?)null,
                        LastPostDate = lastPostDate.HasValue ? this.dateTimeHelper.GetLocalUserDate(lastPostDate.Value) : (DateTime?)null,
                    };

                    details.Add(projectDetails);
                }
            }

            return details;
        }

        public Project GetProject(int id)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var project = this.projectRepository.GetAll().FirstOrDefault(x => x.Id == id);
                if (project == null)
                {
                    return null;
                }
                return new Project()
                    {
                        Id = project.Id,
                        Title = project.Title,
                        Url = project.VkGroup.Url,
                        VkName = project.VkGroup.Name,
                        CreatorId = project.AccountId
                    };
            }
        }

        public CreateProjectResult CreateProject(Project project)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(CONST_CreateProjectQueueName))
            {
                string ticketId = Guid.NewGuid().ToString();
                int accountId = this.currentUserProvider.GetAccountOfCurrentUser().Id;

                CreateProjectCommand command = new CreateProjectCommand
                {
                    AccountId = accountId,
                    Title = project.Title,
                    Url = project.Url,
                    TicketId = ticketId
                };
                
                commandSender.SendCommand(command);
                return new CreateProjectResult { TicketId = ticketId };
            }
        }
        public CreateProjectStatus GetCreateProjectStatus(string ticketId)
        {
            var result = this.DoGetCreateProjectStatus(ticketId);
            
            return result == null
                ? new CreateProjectStatus { TicketId = ticketId, IsFinished = false }
                : new CreateProjectStatus { TicketId = ticketId, IsFinished = true, IsSuccess = result.IsSuccess, ProjectId = result.ProjectId };
        }
        public IList<int> GetConcurentsIdsOf(int projectId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                int accountId = this.currentUserProvider.GetCurrentUser().GetAccount().Id;
                var concurrentIds = this.projectRepository.GetConcurrentIds(accountId, projectId);
                return concurrentIds.ToList();
            }
        }

        public void DeleteProject(int projectId, int groupId)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(CONST_DeleteProjectQueueName))
            {
                try
                {
                    string ticketId = Guid.NewGuid().ToString();
                    DeleteProjectCommand command = new DeleteProjectCommand
                        {
                            ProjectId = projectId,
                            GroupId = groupId,
                            TicketId = ticketId
                        };
                    commandSender.SendCommand(command);
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    var accountId = this.currentUserProvider.GetCurrentUser().GetAccount().Id;
                    var project1 =
                        this.projectRepository.GetByAccountId(accountId).FirstOrDefault(x => x.Id == projectId);
                    if (project1 != null)
                    {
                        this.projectRepository.Delete(project1);
                    }
                }
            }
        }

        private CreateProjectResultCommand DoGetCreateProjectStatus(string ticketId)
        {
            using (ICommandReceiver receiver = Factory.GetInstance<ICommandReceiver>().Open(CONST_CreateProjectResultQueueName, string.Format("TicketId = '{0}'", ticketId)))
            {
                CreateProjectResultCommand result = receiver.GetCommand<CreateProjectResultCommand>();

                if (result != null)
                {
                    result.MarkAsCompleted();
                }

                return result;
            }
        }
    }
}