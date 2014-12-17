namespace Ix.Palantir.Scheduler
{
    using System;
    using System.Threading;

    public class LongCyclesLimiter 
    {
        private const int CONST_WorkCycleThresholMsDefault = 10000;
        private const int CONST_WorkCycleSleepMsDefault = 500;

        private int workThresholdMs;
        private int delayMs;
        private DateTime checkPointTime;

        public LongCyclesLimiter()
        {
            this.workThresholdMs = CONST_WorkCycleThresholMsDefault;
            this.delayMs = CONST_WorkCycleSleepMsDefault;

            this.Reset();
        }

        public LongCyclesLimiter(int workThresholdMs, int sleepMs)
        {
            this.workThresholdMs = workThresholdMs;
            this.delayMs = sleepMs;
            this.Reset();
        }

        public int WorkThresholdMs
        {
            get
            {
                return this.workThresholdMs;
            }

            set
            {
                this.workThresholdMs = value;
            }
        }
        public int DelayMs
        {
            get
            {
                return this.delayMs;
            }

            set
            {
                this.delayMs = value;
            }
        }
        public void CheckPoint()
        {
            if (!this.IsWorkThresholdExceeded())
            {
                return;
            }

            Thread.Sleep(this.delayMs);
            this.Reset();
        }

        public void Reset()
        {
            this.checkPointTime = DateTime.Now;
        }

        private bool IsWorkThresholdExceeded()
        {
            return (DateTime.Now - this.checkPointTime).TotalMilliseconds > this.workThresholdMs;
        }
    }
}