namespace Ix.Palantir.Engine.Services.API
{
    public interface ISchedulingService
    {
        void RunGetVkFeedsProcess();
        void RunProcessVkFeeds();
        void RunEnsureUserInGroups();
        void RunExportDataHandler();
        void RunEnsureFeedJobQueueIsFull();
        void RunEnsureGroupJobQueueIsFull();
        void RunCreateProjectProcess();
        void RunMembersInOutProcess();
    }
}