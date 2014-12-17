namespace Ix.Palantir.Services
{
    using System;
    using DataAccess.API;
    using Infrastructure.Process;

    using Ix.Palantir.Engine.Services.API;
    using Ix.Palantir.Logging;

    using Vkontakte.Workflows.Processes;

    public class SchedulingService : ISchedulingService
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly Func<GetFeedsFromVkProcess> getFeedsProcessFactory;
        private readonly Func<VkDataFeedsParserProcess> processVkFeedsFactory;
        private readonly Func<JoinVkGroupProcess> joinVkGroupProcessFactory;
        private readonly Func<ExportDataProcess> exportDataProcessFactory;
        private readonly Func<CheckFeedJobQueueProcess> checkFeedJobQueueFactory;
        private readonly Func<EnsureGroupJobQueueIsFullProcess> checkGroupJobQueueFactory;
        private readonly Func<CreateProjectProcess> createProjectFactory;
        private readonly Func<MembersInOutUpdateProcess> membersInOutFactory;
        private readonly ILog log;

        public SchedulingService(
            IUnitOfWorkProvider unitOfWorkProvider, 
            Func<GetFeedsFromVkProcess> getFeedsProcessFactory, 
            Func<VkDataFeedsParserProcess> processVkFeedsFactory, 
            Func<JoinVkGroupProcess> joinVkGroupProcessFactory, 
            Func<ExportDataProcess> exportDataProcessFactory, 
            Func<CheckFeedJobQueueProcess> checkFeedJobQueueFactory, 
            Func<EnsureGroupJobQueueIsFullProcess> checkGroupJobQueueFactory, 
            Func<CreateProjectProcess> createProjectFactory, 
            Func<MembersInOutUpdateProcess> membersInOutFactory, 
            ILog log)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.getFeedsProcessFactory = getFeedsProcessFactory;
            this.processVkFeedsFactory = processVkFeedsFactory;
            this.joinVkGroupProcessFactory = joinVkGroupProcessFactory;
            this.exportDataProcessFactory = exportDataProcessFactory;
            this.checkFeedJobQueueFactory = checkFeedJobQueueFactory;
            this.checkGroupJobQueueFactory = checkGroupJobQueueFactory;
            this.createProjectFactory = createProjectFactory;
            this.membersInOutFactory = membersInOutFactory;
            this.log = log;
        }

        public void RunGetVkFeedsProcess()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.getFeedsProcessFactory().ProcessNextQueueItem();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while fetching feeds: {0}", exc));
            }
        }
        public void RunProcessVkFeeds()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.processVkFeedsFactory().ProcessAllFeeds();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while processing feeds: {0}", exc));
            }
        }
        public void RunEnsureUserInGroups()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.joinVkGroupProcessFactory().JoinAllGroups();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while registering user in groups: {0}", exc));
            }
        }
        public void RunExportDataHandler()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.exportDataProcessFactory().ProcessExportQueue();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while exporting data: {0}", exc));
            }
        }
        public void RunEnsureFeedJobQueueIsFull()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.checkFeedJobQueueFactory().Run();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while checking fullness of feed job queue: {0}", exc));
            }
        }
        public void RunEnsureGroupJobQueueIsFull()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.checkGroupJobQueueFactory().Run();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while checking fullness of group job queue: {0}", exc));
            }
        }
        public void RunCreateProjectProcess()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.createProjectFactory().Run();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while creating a project: {0}", exc));
            }
        }
        public void RunMembersInOutProcess()
        {
            try
            {
                using (this.unitOfWorkProvider.CreateUnitOfWork())
                {
                    this.membersInOutFactory().Run();
                }
            }
            catch (Exception exc)
            {
                this.log.ErrorFormat(string.Format("Unhandled exception while creating a project: {0}", exc));
            }
        }
    }
}