namespace Ix.Palantir.Infrastructure.Process
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.Vkontakte.Workflows.Providers;

    public class CreateProjectProcess
    {
        private const string CONST_CreateProjectQueueName = "CreateProjectQueue";
        private const string CONST_CreateProjectResultQueueName = "CreateProjectResultQueue";

        private readonly IProjectRepository projectRepository;
        private readonly IVkGroupRepository vkGroupRepository;
        private readonly IFeedRepository feedRepository;
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IGroupInfoProvider groupInfoProvider;
        private readonly ILog log;

        public CreateProjectProcess(IGroupInfoProvider groupInfoProvider, IVkGroupRepository vkGroupRepository, IUnitOfWorkProvider unitOfWorkProvider, IDateTimeHelper dateTimeHelper, IFeedRepository feedRepository, IProjectRepository projectRepository, ILog log)
        {
            this.groupInfoProvider = groupInfoProvider;
            this.vkGroupRepository = vkGroupRepository;
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.dateTimeHelper = dateTimeHelper;
            this.feedRepository = feedRepository;
            this.projectRepository = projectRepository;
            this.log = log;
        }

        public void Run()
        {
            using (ICommandReceiver receiver = Factory.GetInstance<ICommandReceiver>().Open(CONST_CreateProjectQueueName))
            {
                while (true)
                {
                    CreateProjectCommand createProjectCommand = null;

                    try
                    {
                        createProjectCommand = receiver.GetCommand<CreateProjectCommand>();

                        if (createProjectCommand == null)
                        {
                            this.log.Debug("No create project command found.");
                            return;
                        }

                        var project = this.CreateProject(createProjectCommand);
                        this.SendCreateProjectFinished(createProjectCommand, project, true);
                    }
                    catch (Exception exc)
                    {
                        this.log.ErrorFormat("Exception is occured while creating a project VkUrl {0}, Title {1} for userId = {2}: {3}", createProjectCommand != null ? createProjectCommand.Url : string.Empty, createProjectCommand != null ? createProjectCommand.Title : string.Empty, createProjectCommand != null ? createProjectCommand.AccountId : 0, exc.ToString());

                        if (createProjectCommand != null)
                        {
                            this.SendCreateProjectFinished(createProjectCommand, new Project(), isSuccess: false);
                        }
                    }
                    finally
                    {
                        if (createProjectCommand != null)
                        {
                            createProjectCommand.MarkAsCompleted();
                        }
                    }
                }
            }
        }

        private DomainModel.Project CreateProject(CreateProjectCommand project)
        {
            int vkGroupId = this.groupInfoProvider.GetVkGroupId(project.Url);
            VkGroup group = this.vkGroupRepository.GetGroupById(vkGroupId);
            bool isGroupExists = group != null;

            DomainModel.Project projectToCreate = new DomainModel.Project
            {
                Title = project.Title,
                CreationDate = this.dateTimeHelper.GetDateTimeNow(),
                AccountId = project.AccountId,
                VkGroup = group ?? new VkGroup
                {
                    Id = vkGroupId,
                    Name = this.groupInfoProvider.GetVkGroupName(project.Url),
                    Url = project.Url,
                    Type = VkGroupType.Group
                }
            };

            using (ITransactionScope transaction = this.unitOfWorkProvider.CreateTransaction().Begin())
            {
                this.projectRepository.Save(projectToCreate);

                if (!isGroupExists)
                {
                    this.AddGroupRelatedData(projectToCreate);
                }

                transaction.Commit();
            }

            return projectToCreate;
        }

        private void AddGroupRelatedData(DomainModel.Project projectToCreate)
        {
            IList<long> adminIds = this.groupInfoProvider.GetVkGroupAdministratorIds(projectToCreate.VkGroup.Url, projectToCreate.VkGroup.Type);

            foreach (var adminId in adminIds)
            {
                var admin = new Administrator { UserId = adminId, VkGroupId = projectToCreate.VkGroup.Id };
                this.vkGroupRepository.SaveAdministator(admin);
            }

            var groupAdministrator = new Administrator { UserId = -projectToCreate.VkGroup.Id, VkGroupId = projectToCreate.VkGroup.Id };
            this.vkGroupRepository.SaveAdministator(groupAdministrator);

            this.feedRepository.AddVkGroupFeedFetchingToQueue(projectToCreate.VkGroup.Id);
        }

        private void SendCreateProjectFinished(CreateProjectCommand createProjectCommand, DomainModel.Project project, bool isSuccess)
        {
            Contract.Requires(project != null);

            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(CONST_CreateProjectResultQueueName))
            {
                CreateProjectResultCommand command = new CreateProjectResultCommand
                {
                    VkGroupId = project.VkGroup.Id,
                    AccountId = createProjectCommand.AccountId,
                    TicketId = createProjectCommand.TicketId,
                    IsSuccess = isSuccess,
                    ProjectId = project.Id
                };

                commandSender.SendCommand(command);
            }
        }
    }
}