namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ActionRegistry
    {
        private static readonly ActionRegistry instance = new ActionRegistry();
        private static readonly object lockObject = new object();

        private IList<RunningAction> runningActions = new List<RunningAction>();

        public IList<RunningAction> RunningActions
        {
            get
            {
                return this.runningActions;
            }
        }

        public static ActionRegistry Instance
        {
            get
            {
                return instance;
            }
        }

        public void Add(string name)
        {
            lock (lockObject)
            {
                this.runningActions.Add(new RunningAction(name, DateTime.Now));
            }
        }
        public void Remove(string name)
        {
            lock (lockObject)
            {
                var actionToRemove = this.runningActions.FirstOrDefault(a => string.Compare(a.ActionName, name, StringComparison.InvariantCultureIgnoreCase) == 0);

                if (actionToRemove != null)
                {
                    this.runningActions.Remove(actionToRemove);
                }
            }
        }
    }
}