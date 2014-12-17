namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class EnsureUserInGroups : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService schedulingService;

        public EnsureUserInGroups(ISchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        protected override string ActionName
        {
            get
            {
                return "EnsureUserInGroups";
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
            this.schedulingService.RunEnsureUserInGroups();
        }
    }
}