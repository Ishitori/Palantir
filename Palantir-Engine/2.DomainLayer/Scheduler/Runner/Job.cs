namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Xml;
    using Ix.Palantir.Logging;

    public class Job
    {
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public event OnLogActionHandler LogAction;

        internal Job(Scheduler scheduler, string name)
        {
            this.mName = null;
            this.mTriggerNameTrigger = new Hashtable();
            this.mTriggers = new Hashtable();
            this.mTimeTriggers = new SortedList();
            this.mScheduler = null;
            this.mState = JobState.Uninitialized;
            this.mSynchObject = new object();
            this.LogAction = null;
            this.mScheduler = scheduler;
            this.mName = name;
        }

        private void AddTrigger(Trigger trigger)
        {
            this.mTriggers[trigger] = null;
        }
        internal void Fire(DateTime now)
        {
            lock (this.mSynchObject)
            {
                if (this.mState != JobState.Initialized)
                {
                    throw new InvalidOperationException("Job not initialized.");
                }

                ArrayList list1 = (ArrayList)this.mTimeTriggers.GetByIndex(0);
                this.LogTriggersState("OnBeforeFire");
                log.Info("Executing job \"" + this.mName + "\".");
                
                foreach (Trigger trigger1 in list1)
                {
                    if (trigger1.State == TriggerState.Initialized)
                    {
                        trigger1.Fire(now);
                    }
                }

                this.mTimeTriggers.RemoveAt(0);
                this.LogTriggersState("OnAfterFireWithRemove");
                foreach (Trigger trigger2 in list1)
                {
                    long num1 = trigger2.GetNextTriggerTime();
                    this.UpdateTriggerTimes(trigger2, num1);
                }

                this.LogTriggersState("OnAfterRecalculating");
            }
        }

        public long GetNextScheduledTime()
        {
            long num1 = -1;
            lock (this.mSynchObject)
            {
                if (this.mTimeTriggers.Count != 0)
                {
                    num1 = (long)this.mTimeTriggers.GetKey(0);
                }

                return num1;
            }
        }

        public ArrayList GetTriggers()
        {
            lock (this.mSynchObject)
            {
                return new ArrayList(this.mTriggers.Keys);
            }
        }

        internal void Init(long ticks_in)
        {
            lock (this.mSynchObject)
            {
                if (this.mState == JobState.Uninitialized)
                {
                    if (this.mTriggers.Count == 0)
                    {
                        throw new InvalidOperationException("Job contains no triggers.");
                    }

                    log.Info("Initializing. Job: " + this.mName);
                    ArrayList list1 = new ArrayList(this.mTriggers.Keys);
                    foreach (Trigger trigger1 in list1)
                    {
                        trigger1.Init(ticks_in);
                        long num1 = trigger1.GetNextTriggerTime();
                        this.UpdateTriggerTimes(trigger1, num1);
                    }

                    this.mState = JobState.Initialized;
                }
            }
        }
        public void InitializeNewTriggers()
        {
            long num1 = -1;
            lock (this.mSynchObject)
            {
                if (this.mState != JobState.Initialized)
                {
                    throw new InvalidOperationException("This method cannot be called before the job has been initialized.");
                }

                log.Info("Initializing new triggers. Job: " + this.mName);
                long num2 = DateTime.Now.Ticks;
                ArrayList list1 = new ArrayList(this.mTriggers.Keys);
                foreach (Trigger trigger1 in list1)
                {
                    if (trigger1.State == TriggerState.Uninitialized)
                    {
                        trigger1.Init(num2);
                        long num3 = trigger1.GetNextTriggerTime();
                        this.UpdateTriggerTimes(trigger1, num3);
                        num1 = (long)this.mTimeTriggers.GetKey(0);
                        if (num1 < num3)
                        {
                            num1 = -1;
                        }
                    }
                }
            }

            if (num1 >= 0)
            {
                this.mScheduler.UpdateNextFiringTime(num1, this);
            }
        }

        public CronTrigger MakeCronTrigger(string name)
        {
            lock (this.mSynchObject)
            {
                return (CronTrigger)this.MakeTrigger(TriggerType.CronTrigger, name);
            }
        }
        public TimerTrigger MakeTimerTrigger(string name)
        {
            lock (this.mSynchObject)
            {
                return (TimerTrigger)this.MakeTrigger(TriggerType.TimerTrigger, name);
            }
        }

        private Trigger MakeTrigger(TriggerType triggerType, string name)
        {
            if (this.mTriggerNameTrigger.ContainsKey(name))
            {
                throw new ArgumentException("Trigger with the name \"" + name + "\" already exists.");
            }

            Trigger trigger1 = null;
            switch (triggerType)
            {
                case TriggerType.TimerTrigger:
                    trigger1 = new TimerTrigger(name);
                    break;

                case TriggerType.CronTrigger:
                    trigger1 = new CronTrigger(name);
                    break;
            }

            this.AddTrigger(trigger1);
            this.mTriggerNameTrigger[name] = trigger1;
            log.Info("Made " + triggerType.ToString() + " with name \"" + name + "\". Job: " + this.mName);
            trigger1.LogAction += new Trigger.OnLogActionHandler(this.Ret_LogAction);
            return trigger1;
        }

        private void OnLogAction(string msg)
        {
            if (this.LogAction != null)
            {
                this.LogAction(this, new LogActionEventArgs(msg));
            }
        }

        internal Hashtable ParseXML(XmlNode node)
        {
            XmlNodeList list1 = node.SelectNodes("./Trigger");
            foreach (XmlElement element1 in list1)
            {
                TriggerType type1 = (TriggerType)Array.IndexOf(Enum.GetNames(typeof(TriggerType)), element1.GetAttribute("type"));
                string text1 = element1.GetAttribute("name");
                this.MakeTrigger(type1, text1).ParseXML(element1);
            }

            return this.mTriggerNameTrigger;
        }

        public void RemoveTrigger(Trigger trigger)
        {
            long num1 = -1;
            bool flag1 = false;
            lock (this.mSynchObject)
            {
                if (!this.mTriggers.ContainsKey(trigger))
                {
                    return;
                }

                if (this.mTriggers[trigger] != null)
                {
                    num1 = (long)this.mTriggers[trigger];
                    ArrayList list1 = (ArrayList)this.mTimeTriggers[num1];
                    list1.Remove(trigger);
                    if (list1.Count == 0)
                    {
                        this.mTimeTriggers.Remove(num1);
                    }
                }

                if (this.mTimeTriggers.Count == 0)
                {
                    flag1 = true;
                }
                else
                {
                    num1 = (long)this.mTimeTriggers.GetKey(0);
                }

                this.mTriggers.Remove(trigger);
                log.Info("Removed trigger \"" + trigger.Name + "\". Job: " + this.mName);
            }

            if (this.mState == JobState.Initialized)
            {
                if (flag1)
                {
                    this.mScheduler.RemoveJob(this);
                }
                else
                {
                    this.mScheduler.UpdateNextFiringTime(num1, this);
                }
            }
        }

        private void Ret_LogAction(Trigger sender, LogActionEventArgs args)
        {
            this.OnLogAction(args.Message);
        }

        private void UpdateTriggerTimes(Trigger trigger, long ticks)
        {
            if (ticks < 0)
            {
                this.mTriggers[trigger] = null;
            }
            else
            {
                this.mTriggers[trigger] = ticks;
                ArrayList list1 = (ArrayList)this.mTimeTriggers[ticks];
                if (list1 == null)
                {
                    list1 = new ArrayList();
                    this.mTimeTriggers[ticks] = list1;
                }

                list1.Add(trigger);
            }
        }
        private void LogTriggersState(string header)
        {
            logItems.DebugFormat("{0}.{1}", this.Name, header);

            foreach (long tick in this.mTimeTriggers.Keys)
            {
                logItems.DebugFormat("{0} - {1}", tick, this.GetTriggerNames((ArrayList)this.mTimeTriggers[tick]));
            }
        }
        private string GetTriggerNames(ArrayList triggers)
        {
            StringBuilder triggerDisplay = new StringBuilder();

            foreach (Trigger trigger in triggers)
            {
                triggerDisplay.AppendFormat("{0}, ", trigger.Name);
            }

            if (triggerDisplay.Length > 0)
            {
                triggerDisplay.Remove(triggerDisplay.Length - 2, 2);
            }

            return triggerDisplay.ToString();
        }

        public string Name
        {
            get
            {
                return this.mName;
            }
        }

        public JobState State
        {
            get
            {
                lock (this.mSynchObject)
                {
                    return this.mState;
                }
            }
        }

        private static readonly ILog log = LogManager.GetLogger("Scheduler");
        private static readonly ILog logItems = LogManager.GetLogger("SchedulerItems");
        private string mName;
        private Scheduler mScheduler;
        private volatile JobState mState = JobState.Uninitialized;
        private object mSynchObject;
        private SortedList mTimeTriggers;
        private Hashtable mTriggerNameTrigger;
        private Hashtable mTriggers;

        private delegate void FireTrigger(DateTime now);

        public delegate void OnLogActionHandler(Job sender, LogActionEventArgs args);
    }
}