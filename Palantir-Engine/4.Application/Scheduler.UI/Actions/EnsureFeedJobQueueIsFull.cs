namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class EnsureFeedJobQueueIsFull : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService schedulingService;

        public EnsureFeedJobQueueIsFull(ISchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        protected override string ActionName
        {
            get
            {
                return "EnsureFeedJobQueueIsFull";
            }
        }
        protected override object SyncRoot
        {
            get
            {
                return syncRoot;
            }
        }

        protected override void DoAction()
        {
            this.schedulingService.RunEnsureFeedJobQueueIsFull();
        }
    }
}