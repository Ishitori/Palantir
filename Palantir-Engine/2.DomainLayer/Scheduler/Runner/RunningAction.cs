namespace Ix.Palantir.Scheduler.Runner
{
    using System;

    public class RunningAction
    {
        private DateTime startTime;
        private string actionName;

        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }

            set
            {
                this.startTime = value;
            }
        }
        public string ActionName
        {
            get
            {
                return this.actionName;
            }

            set
            {
                this.actionName = value;
            }
        }

        public RunningAction(string name, DateTime startTime)
        {
            this.actionName = name;
            this.startTime = startTime;
        }
    }
}