namespace Ix.Palantir.Scheduler.UI.Actions
{
    using Ix.Palantir.Engine.Services.API;

    public class MembersInOutUpdateAction : ScheduleActionBase
    {
        private static readonly object syncRoot = new object();
        private readonly ISchedulingService process;

        public MembersInOutUpdateAction(ISchedulingService process)
        {
            this.process = process;
        }

        protected override string ActionName
        {
            get
            {
                return "MembersInOutUpdateAction";
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
            this.process.RunMembersInOutProcess();
        }
    }
}