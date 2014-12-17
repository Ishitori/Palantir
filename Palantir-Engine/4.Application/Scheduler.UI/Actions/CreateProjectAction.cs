namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class CreateProjectAction : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService schedulingService;

        public CreateProjectAction(ISchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        protected override string ActionName
        {
            get
            {
                return "CreateProjectProcess";
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
            this.schedulingService.RunCreateProjectProcess();
        }
    }
}