namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class EnsureGroupJobQueueIsFull : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService schedulingService;

        public EnsureGroupJobQueueIsFull(ISchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        protected override string ActionName
        {
            get
            {
                return "EnsureGroupJobQueueIsFull";
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
            this.schedulingService.RunEnsureGroupJobQueueIsFull();
        }
    }
}