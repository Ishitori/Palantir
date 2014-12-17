namespace Ix.Palantir.Scheduler.Runner
{
    using System.Collections;

    public class ActionContext
    {
        internal ActionContext(AbstractAction action, Trigger trigger, Hashtable properties)
        {
            this.mAction = action;
            this.mTrigger = trigger;
            this.mProperties = properties;
        }

        public ActionResult GetLastResult()
        {
            return this.mTrigger.GetLastResult(this.mAction);
        }

        public AbstractAction Action
        {
            get
            {
                return this.mAction;
            }
        }
        public Hashtable Properties
        {
            get
            {
                return this.mProperties;
            }
        }
        public Trigger Trigger
        {
            get
            {
                return this.mTrigger;
            }
        }

        private AbstractAction mAction;
        private Hashtable mProperties;
        private Trigger mTrigger;
    }
}