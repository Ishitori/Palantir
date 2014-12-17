namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class ProcessVkFeedsAction : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService process;

        public ProcessVkFeedsAction(ISchedulingService process)
        {
            this.process = process;
        }

        protected override string ActionName
        {
            get
            {
                return "ProcessVkFeeds";
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
            this.process.RunProcessVkFeeds();
        }
    }
}