namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using Exceptions;
    using Framework.ObjectFactory;
    using Ix.Palantir.Logging;

    public class Scheduler : IDisposable
    {
        private const string CONST_TriggerTimeFiringOffset = "Scheduler.TriggerTimeFiringOffset";

        private bool disposed;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Reviewed. Suppression is OK here.")]
        static Scheduler()
        {
            if (ConfigurationManager.AppSettings[CONST_TriggerTimeFiringOffset] == null)
            {
                triggerTimeFiringOffset = 0;
            }
            else
            {
                int.TryParse(ConfigurationManager.AppSettings[CONST_TriggerTimeFiringOffset], out triggerTimeFiringOffset);
            }
        }

        public Scheduler(string name) : this(name, false)
        {
        }
        public Scheduler(string name, bool logEvents)
        {
            this.mJobTime = new Hashtable();
            this.mJobNameJob = new Hashtable();
            this.mTimeJobs = new SortedList();
            this.mTimer = null;
            this.mName = null;
            this.mTimerHackInUse = false;
            this.mSynchObject = new object();
            this.mState = 0;
            this.mEventLoggingEnabled = true;
            this.mEventLoggingEnabled = logEvents;
            if (this.mEventLoggingEnabled)
            {
                try
                {
                    this.mEventLog = new EventLog("Application");
                    this.mEventLog.Source = "Palantir.Scheduler";
                }
                catch (Exception ex)
                {
                    if (ExceptionHelper.IsFatalException(ex))
                    {
                        throw;
                    }

                    this.mEventLoggingEnabled = false;
                }
            }

            this.mName = name;
            this.mTimer = new Timer(this.Fire, null, -1, -1);
            log.Info("Created scheduler with name \"" + this.mName + "\"");
            this.LogAction("Created scheduler with name \"" + this.Name);
        }

        public Job CreateJob(string jobName)
        {
            lock (this.mSynchObject)
            {
                if (this.mState == SchedulerState.Stopped)
                {
                    throw new InvalidOperationException("Cannot create job for a  stopped scheduler.");
                }

                if (this.mJobNameJob.ContainsKey(jobName))
                {
                    throw new ArgumentException("Job with the name \"" + jobName + "\" already exists.");
                }

                Job job1 = new Job(this, jobName);
                job1.LogAction += new Job.OnLogActionHandler(this.Ret_LogAction);
                this.mJobNameJob[jobName] = job1;
                this.mJobTime[job1] = null;
                log.Info("Created new job with name: \"" + job1.Name + "\". Scheduler: " + this.mName);
                return job1;
            }
        }

        private void Fire(object obj)
        {
            lock (this.mSynchObject)
            {
                if (this.mState == SchedulerState.Stopped)
                {
                    throw new InvalidOperationException("Cannot fire stopped scheduler.");
                }

                if (this.mState == SchedulerState.Started)
                {
                    if (this.mTimerHackInUse)
                    {
                        this.UpdateTimer();
                    }
                    else
                    {
                        long num1 = (long)this.mTimeJobs.GetKey(0);
                        log.Info("Executing jobs. Scheduler: " + this.mName);
                        this.StartLogItems("OnBefore.Scheduler.Fire - UpdateNextFiringTime");
                        this.EndLogItems();
                        DateTime time1 = DateTime.Now;
                        ArrayList list1 = new ArrayList((ArrayList)this.mTimeJobs[num1]);
                        this.mTimeJobs.Remove(num1);
                        foreach (Job job1 in list1)
                        {
                            job1.Fire(time1);
                            this.LogAction("Fired job " + job1.Name);
                        }

                        this.StartLogItems("OnAfter.Scheduler.Fire - UpdateNextFiringTime");
                        foreach (Job job2 in list1)
                        {
                            long num2 = job2.GetNextScheduledTime();
                            if (num2 < 0)
                            {
                                this.RemoveJob(job2);
                                this.mJobTime.Remove(job2);
                            }
                            else
                            {
                                this.UpdateNextFiringTime(num2, job2, false);
                            }
                        }

                        this.EndLogItems();
                        this.UpdateTimer();
                    }
                }
            }
        }

        public ArrayList GetJobs()
        {
            lock (this.mSynchObject)
            {
                return new ArrayList(this.mJobTime.Keys);
            }
        }

        private void Go()
        {
            lock (this.mSynchObject)
            {
                long num1 = DateTime.Now.Ticks;
                log.Info("Starting scheduler " + this.mName + ".");
                log.Info("Initializing jobs. Scheduler " + this.mName + ".");
                foreach (Job job1 in this.mJobTime.Keys)
                {
                    job1.Init(num1);
                }

                this.mState = SchedulerState.Started;
                ArrayList list1 = new ArrayList(this.mJobTime.Keys);
                
                this.StartLogItems("Scheduler.go - UpdateNextFiringTime");
                foreach (Job job2 in list1)
                {
                    this.UpdateNextFiringTime(job2.GetNextScheduledTime(), job2, false);
                }

                this.EndLogItems();

                this.UpdateTimer();
            }
        }

        [Obsolete]
        public void InitializeNewJobs()
        {
            lock (this.mSynchObject)
            {
                if (this.mState != SchedulerState.Started)
                {
                    throw new InvalidOperationException("Scheduler cannot intialize new jobs because the state is: " + this.mState.ToString() + ".");
                }

                log.Info("Initializing new jobs. Scheduler: " + this.mName);
                ArrayList list1 = new ArrayList();
                long num1 = DateTime.Now.Ticks;
                foreach (Job job1 in this.mJobTime.Keys)
                {
                    if (job1.State == JobState.Uninitialized)
                    {
                        list1.Add(job1);
                        job1.Init(num1);
                    }
                }

                foreach (Job job2 in list1)
                {
                    this.UpdateNextFiringTime(job2.GetNextScheduledTime(), job2, false);
                }

                this.UpdateTimer();
            }
        }
        public void KillJob(Job job)
        {
            lock (this.mSynchObject)
            {
                this.RemoveJob(job);
                this.mJobTime.Remove(job);
                this.UpdateTimer();
                log.Info("Killed job: \"" + job.Name + "\". Scheduler: " + this.mName);
            }
        }

        public void LogAction(string msg)
        {
            if (this.mEventLoggingEnabled)
            {
                try
                {
                    this.mEventLog.WriteEntry(msg);
                }
                catch (Exception ex)
                {
                    if (ExceptionHelper.IsFatalException(ex))
                    {
                        throw;
                    }

                    this.mEventLoggingEnabled = false;
                }
            }
        }

        public static Scheduler MakeFromXML(string[] assemblyPaths, XmlNode node)
        {
            XmlElement xmlNode = (XmlElement)node;

            string text1 = xmlNode.GetAttribute("name");
            bool flag1 = false;
            string text2 = xmlNode.GetAttribute("eventLogging");
            if ("true".Equals(text2))
            {
                flag1 = true;
            }

            Scheduler scheduler1 = new Scheduler(text1, flag1);
            Hashtable hashtable1 = new Hashtable();
            Assembly assembly1 = scheduler1.GetType().Assembly;
            string text3 = scheduler1.GetType().Namespace;
            XmlNodeList list1 = node.SelectNodes("./Jobs/Job");
            foreach (XmlElement element1 in list1)
            {
                string text4 = element1.GetAttribute("name");
                hashtable1[text4] = scheduler1.CreateJob(text4).ParseXML(element1);
            }

            XmlNodeList list2 = node.SelectNodes("./Actions/Action");
            foreach (XmlElement element2 in list2)
            {
                string text5 = text3 + "." + element2.GetAttribute("type");
                string text6 = element2.GetAttribute("name");
                AbstractAction action1 = (AbstractAction)Activator.CreateInstance(Type.GetType(text5 + ", " + assembly1.FullName), new object[] { text6 });
                action1.ParseXML(assemblyPaths, element2);
                XmlNodeList list3 = element2.SelectNodes("./Associations/AssociateWith");
                foreach (XmlElement element3 in list3)
                {
                    string text7 = element3.GetAttribute("job");
                    string text8 = element3.GetAttribute("trigger");
                    if (!hashtable1.ContainsKey(text7))
                    {
                        throw new ArgumentException("Cannot find the job \"" + text7 + "\".");
                    }

                    Hashtable hashtable2 = (Hashtable)hashtable1[text7];
                    if (!hashtable2.ContainsKey(text8))
                    {
                        throw new ArgumentException("Cannot find the trigger \"" + text8 + "\" in the job \"" + text7 + "\".");
                    }

                    Trigger trigger1 = (Trigger)hashtable2[text8];
                    trigger1.AddAction(action1);
                    XmlNodeList list4 = element3.SelectNodes("./Properties/Property");
                    if (list4.Count != 0)
                    {
                        Hashtable hashtable3 = new Hashtable();
                        foreach (XmlElement element4 in list4)
                        {
                            if (!element4.HasAttribute("key"))
                            {
                                throw new ArgumentException("Missing property key for the action \"" + text6 + "\" associated with the job \"" + text7 + "\" and the trigger \"" + text8 + "\"");
                            }

                            string text9 = element4.GetAttribute("key");
                            string text10 = element4.GetAttribute("value");
                            hashtable3[text9] = text10;
                        }

                        trigger1.SetActionProperties(action1, hashtable3);
                    }
                }
            }

            return scheduler1;
        }

        internal void RemoveJob(Job job)
        {
            lock (this.mSynchObject)
            {
                if (this.mState == SchedulerState.Started)
                {
                    this.mTimer.Change(-1, -1);
                    if (this.mJobTime.ContainsKey(job) && (this.mJobTime[job] != null))
                    {
                        long num1 = (long)this.mJobTime[job];
                        if (this.mTimeJobs.ContainsKey(num1))
                        {
                            ArrayList list1 = (ArrayList)this.mTimeJobs[num1];
                            list1.Remove(job);
                            if (list1.Count == 0)
                            {
                                this.mTimeJobs.Remove(num1);
                            }
                        }
                    }
                }
            }
        }

        private void Ret_LogAction(Job sender, LogActionEventArgs args)
        {
            this.LogAction(args.Message);
        }

        public void Start()
        {
            lock (this.mSynchObject)
            {
                if (this.mState != SchedulerState.Ready)
                {
                    throw new InvalidOperationException("Scheduler cannot be started in the current state. Current scheduler state: \"" + this.mState.ToString() + "\"");
                }

                Thread thread = new Thread(new ThreadStart(this.Go));
                thread.Priority = ThreadPriority.Lowest;
                thread.Start();
            }
        }
        public void Stop()
        {
            lock (this.mSynchObject)
            {
                this.mState = SchedulerState.Stopped;
                this.mTimer.Change(-1, -1);
            }
        }

        [Obsolete]
        internal void UpdateNextFiringTime(long ticks, Job job)
        {
            this.UpdateNextFiringTime(ticks, job, true);
        }
        private void UpdateNextFiringTime(long ticks, Job job, bool updateTimer)
        {
            lock (this.mSynchObject)
            {
                if (this.mState == SchedulerState.Started)
                {
                    this.mTimer.Change(-1, -1);
                    if (this.mJobTime.ContainsKey(job) && (this.mJobTime[job] != null))
                    {
                        long num1 = (long)this.mJobTime[job];
                        if (this.mTimeJobs.ContainsKey(num1))
                        {
                            ArrayList list1 = (ArrayList)this.mTimeJobs[num1];
                            list1.Remove(job);
                            if (list1.Count == 0)
                            {
                                this.mTimeJobs.Remove(num1);
                            }
                        }
                    }

                    this.mJobTime[job] = ticks;
                    ArrayList list2 = (ArrayList)this.mTimeJobs[ticks];
                    if (list2 == null)
                    {
                        list2 = new ArrayList();
                        list2.Add(job);
                        this.mTimeJobs[ticks] = list2;
                    }
                    else
                    {
                        if (list2.Contains(job))
                        {
                            log.ErrorFormat("Job list for ticks {0} already contains job \"{1}\"", ticks, job.Name);
                        }

                        list2.Add(job);
                    }

                    if (updateTimer)
                    {
                        this.UpdateTimer();
                    }
                }
            }
        }

        private void StartLogItems(string info)
        {
            logItems.Debug(info + " Ticks: " + DateTime.Now.Ticks);
        }

        private string GetFormattedJobList(ArrayList jobs)
        {
            StringBuilder jobsDisplay = new StringBuilder();

            foreach (Job job in jobs)
            {
                jobsDisplay.AppendFormat("{0}, ", job.Name);
            }

            if (jobsDisplay.Length > 0)
            {
                jobsDisplay.Remove(jobsDisplay.Length - 2, 2);
            }

            return jobsDisplay.ToString();
        }

        private void EndLogItems()
        {
            logItems.Debug("Scheduler.UpdateNextFiringTime is called. Jobs collection is:");
            foreach (long tick in this.mTimeJobs.Keys)
            {
                logItems.DebugFormat("{0} - {1}", tick, this.GetFormattedJobList((ArrayList)this.mTimeJobs[tick]));
            }
        }

        private void UpdateTimer()
        {
            lock (this.mSynchObject)
            {
                if ((this.mState == SchedulerState.Started) && (this.mTimeJobs.Count != 0))
                {
                    long num1 = (long)this.mTimeJobs.GetKey(0);
                    long num2 = ((num1 - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond) + triggerTimeFiringOffset; // 10 000 I add offset to prevent round error in division.
                    logItems.DebugFormat("UpdateTimer. num1 = {0}, ticks = {1} num2 = {2} ", num1, DateTime.Now.Ticks, num2);
                    if (num2 > 0x7ffffffd)
                    {
                        // 2 147 483 645
                        this.mTimerHackInUse = true;
                        num2 = 0x7ffffffd;
                    }
                    else
                    {
                        this.mTimerHackInUse = false;
                    }

                    logItems.DebugFormat("UpdateTimer. New timer params: {0}, {1} ", (num2 > 0) ? num2 : 0, this.mTimerHackInUse);
                    this.mTimer.Change((num2 > 0) ? num2 : 0, -1); // -1 = Infinite (no periods)
                }
            }
        }

        public bool EventLoggingEnabled
        {
            get
            {
                return this.mEventLoggingEnabled;
            }

            set
            {
                this.mEventLoggingEnabled = value;
            }
        }

        public string Name
        {
            get
            {
                return this.mName;
            }
        }

        public SchedulerState State
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
        private static int triggerTimeFiringOffset;
        private EventLog mEventLog;
        private bool mEventLoggingEnabled = false;
        private Hashtable mJobNameJob;
        private Hashtable mJobTime;
        private string mName;
        private volatile SchedulerState mState;
        private object mSynchObject;
        private volatile SortedList mTimeJobs;
        private Timer mTimer;
        private volatile bool mTimerHackInUse;

        internal delegate void JobFiringDelegate(DateTime time);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mEventLog", Justification = "Reviewed. Suppression is OK here."), SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "mTimer", Justification = "Reviewed. Suppression is OK here.")]
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.mEventLog != null)
                    {
                        this.mEventLog.Dispose();
                    }

                    if (this.mTimer != null)
                    {
                        this.mTimer.Dispose();
                    }
                }

                this.disposed = true;
            }
        }
    }
}