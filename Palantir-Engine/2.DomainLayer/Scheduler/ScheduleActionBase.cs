namespace Ix.Palantir.Scheduler
{
    using System;
    using System.Threading;
    using Ix.Palantir.Logging;
    using Runner;

    public abstract class ScheduleActionBase : IActionListener
    {
        private const string CONST_LogStartedMessage = "Started - {0}";
        private const string CONST_LogFinishedMessage = "Finished - {0}";
        private const string CONST_LogFailedMessage = "Failed - {0}. Exception: {1}";

        private ILog log;
        private LongCyclesLimiter workLimiter;
        private ActionContext actionContext;

        protected ILog Log
        {
            get
            {
                return this.log;
            }

            set
            {
                this.log = value;
            }
        }
        protected ActionContext ActionContext
        {
            get
            {
                return this.actionContext;
            }
        }
        protected abstract string ActionName
        {
            get;
        }
        protected abstract object SyncRoot
        { 
            get;
        }
        protected virtual bool WaitInQueueIfInstanceIsAlreadyRunning
        {
            get
            {
                return false;
            }
        }

        public object FireAction(ActionContext key)
        {
            this.Init(key);

            try
            {
                if (this.SyncRoot == null)
                {
                    this.Log.InfoFormat(CONST_LogStartedMessage, this.ActionName);
                    this.DoAction();
                    this.Log.InfoFormat(CONST_LogFinishedMessage, this.ActionName);
                    return this;
                }

                if (this.WaitInQueueIfInstanceIsAlreadyRunning)
                {
                    lock (this.SyncRoot)
                    {
                        this.Log.InfoFormat(CONST_LogStartedMessage, this.ActionName);
                        this.DoAction();
                        this.Log.InfoFormat(CONST_LogFinishedMessage, this.ActionName);
                    }
                }
                else
                {
                    if (Monitor.TryEnter(this.SyncRoot, TimeSpan.FromSeconds(3)))
                    {
                        try
                        {
                            this.Log.InfoFormat(CONST_LogStartedMessage, this.ActionName);
                            this.DoAction();
                            this.Log.InfoFormat(CONST_LogFinishedMessage, this.ActionName);
                        }
                        finally
                        {
                            Monitor.Exit(this.SyncRoot);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.Log.ErrorFormat(CONST_LogFailedMessage, this.ActionName, exc.ToString());

                if (exc is ThreadAbortException)
                {
                    throw;
                }
            }

            return this;
        }

        protected abstract void DoAction();
        protected void WorkLimitCheckPoint()
        {
            this.workLimiter.CheckPoint();
        }

        protected bool CheckIfMoreThanDaysDiff(int days, DateTime? maxDate, DateTime? minDate)
        {
            if (maxDate == null || minDate == null)
            {
                return true;
            }

            return (maxDate.Value - minDate.Value).TotalDays >= days;
        }
        protected bool CheckIfMoreThanDaysDiff(int days, DateTime? checkDate)
        {
            return this.CheckIfMoreThanDaysDiff(days, checkDate, DateTime.Now);
        }

        private void Init(ActionContext key)
        {
            this.actionContext = key;
            this.log = LogManager.GetLogger("Scheduler");
            this.workLimiter = new LongCyclesLimiter(); // use app config settings
        } 
    }
}