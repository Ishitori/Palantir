namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using Ix.Palantir.Logging;

    public abstract class Trigger
    {
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public event OnLogActionHandler LogAction;

        protected Trigger()
        {
            this.InternalName = null;
            this.internalState1 = 0;
            this.Actions = new ArrayList();
            this.ActionNameAction = new Hashtable();
            this.LastTriggerTime = -1;
            this.NextTriggerTime = -1;
            this.InternalErrorMessage = string.Empty;
            this.ActionPropertiesMap = new Hashtable();
            this.ActionLastResultMap = new Hashtable();
            this.SynchObject = new object();
            this.LogAction = null;
        }

        protected virtual void ActionCallback(IAsyncResult res)
        {
            object[] objArray1 = (object[])res.AsyncState;
            AbstractAction action1 = (AbstractAction)objArray1[0];
            ActionResult result1 = ((ExecuteActionDelegate)objArray1[1]).EndInvoke(res);
            lock (this.SynchObject)
            {
                this.ActionLastResultMap[action1] = result1;
            }

            this.LogEvent("Action \"" + action1.Name + "\" returned (action state is " + result1.State.ToString() + ". Trigger \"" + this.InternalName + "\").");
            ActionRegistry.Instance.Remove(action1.Name);
        }

        public virtual void AddAction(AbstractAction action)
        {
            lock (this.SynchObject)
            {
                if (this.ActionNameAction.ContainsKey(action.Name))
                {
                    log.Error("Action with the name \"" + action.Name + "\" is already associated with the trigger \"" + this.InternalName + "\".");
                    throw new ArgumentException("Action with the name \"" + action.Name + "\" is already associated with the trigger \"" + this.InternalName + "\".");
                }

                this.ActionNameAction[action.Name] = action;
                this.Actions.Add(action);
                this.ActionPropertiesMap[action] = null;
                this.ActionLastResultMap[action] = null;
                this.LogEvent("Added action \"" + action.Name + "\". Trigger \"" + this.InternalName + "\".");
            }
        }

        protected internal virtual void Fire(DateTime now)
        {
            foreach (AbstractAction action1 in this.Actions)
            {
                ExecuteActionDelegate delegate1 = action1.Execute;
                Hashtable hashtable1 = (Hashtable)this.ActionPropertiesMap[action1];
                delegate1.BeginInvoke(new ActionContext(action1, this, hashtable1), new AsyncCallback(this.ActionCallback), new object[] { action1, delegate1 });
                this.LogEvent("Fired action \"" + action1.Name + "\". Trigger \"" + this.InternalName + "\".");
                ActionRegistry.Instance.Add(action1.Name);
            }
        }

        public virtual ArrayList GetActions()
        {
            lock (this.SynchObject)
            {
                return (ArrayList)this.Actions.Clone();
            }
        }

        public virtual ActionResult GetLastResult(AbstractAction action)
        {
            lock (this.SynchObject)
            {
                return (ActionResult)this.ActionLastResultMap[action];
            }
        }

        public virtual long GetLastTriggerTime()
        {
            lock (this.SynchObject)
            {
                return this.LastTriggerTime;
            }
        }

        public virtual long GetNextTriggerTime()
        {
            lock (this.SynchObject)
            {
                return this.NextTriggerTime;
            }
        }

        protected internal abstract void Init(long ticks);
        private void LogEvent(string msg)
        {
            log.Info(msg);
            this.OnLogAction(msg);
        }

        private void OnLogAction(string msg)
        {
            if (this.LogAction != null)
            {
                this.LogAction(this, new LogActionEventArgs(msg));
            }
        }

        internal abstract void ParseXML(XmlNode node);
        public virtual bool RemoveAction(AbstractAction action)
        {
            bool flag1 = false;
            lock (this.SynchObject)
            {
                flag1 = this.ActionNameAction.ContainsKey(action.Name);
                if (flag1)
                {
                    this.Actions.Remove(action);
                    this.ActionNameAction.Remove(action.Name);
                    this.ActionPropertiesMap.Remove(action);
                    this.ActionLastResultMap.Remove(action);
                    this.LogEvent("Removeded action \"" + action.Name + "\". Trigger \"" + this.InternalName + "\".");
                }

                return flag1;
            }
        }

        public virtual void SetActionProperties(AbstractAction action, Hashtable properties)
        {
            lock (this.SynchObject)
            {
                this.ActionPropertiesMap[action] = properties;
            }
        }

        public virtual string ErrorMessage
        {
            get
            {
                return this.InternalErrorMessage;
            }
        }

        public virtual string Name
        {
            get
            {
                return this.InternalName;
            }
        }

        public virtual TriggerState State
        {
            get
            {
                lock (this.SynchObject)
                {
                    return this.internalState1;
                }
            }
        }

        private static readonly ILog log = LogManager.GetLogger("Scheduler");
        protected internal Hashtable ActionLastResultMap { get; set; }
        protected internal Hashtable ActionNameAction { get; set; }
        protected internal Hashtable ActionPropertiesMap { get; set; }
        protected internal ArrayList Actions { get; set; }
        protected internal string InternalErrorMessage { get; set; }
        protected internal long LastTriggerTime { get; set; }
        protected internal string InternalName { get; set; }
        protected internal long NextTriggerTime { get; set; }
        protected internal TriggerState InternalState
        {
            get
            {
                return this.internalState1;
            }

            set
            {
                this.internalState1 = value;
            }
        }
        private volatile TriggerState internalState1 = TriggerState.Uninitialized;
        protected internal object SynchObject { get; set; }

        protected internal delegate ActionResult ExecuteActionDelegate(ActionContext context);

        public delegate void OnLogActionHandler(Trigger sender, LogActionEventArgs args);
    }
}