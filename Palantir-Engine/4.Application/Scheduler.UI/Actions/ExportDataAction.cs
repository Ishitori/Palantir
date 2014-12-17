namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class ExportDataAction : ScheduleActionBase
    {
        private static readonly object SyncRootObject = new object();
        private readonly ISchedulingService schedulingService;

        public ExportDataAction(ISchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        protected override string ActionName
        {
            get
            {
                return "ExportDataAction";
            }
        }
        protected override object SyncRoot
        {
            get
            {
                return SyncRootObject;
            }
        }

        protected override void DoAction()
        {
            this.schedulingService.RunExportDataHandler();
        }
    }
}