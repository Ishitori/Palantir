namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class GetFeedsFromVkAction : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService schedulingService;

        public GetFeedsFromVkAction(ISchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        protected override string ActionName
        {
            get
            {
                return "GetFeedsFromVk";
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
            this.schedulingService.RunGetVkFeedsProcess();
        }
    }
}