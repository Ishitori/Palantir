namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Ix.Palantir.Logging;

    public class TimerTrigger : Trigger
    {
        protected internal TimerTrigger(string name)
        {
            this.mSecondsInterval = 0;
            this.mMinutesInterval = 0;
            this.mHoursInterval = 0;
            this.mDaysInterval = 0;
            this.mMonthsInterval = 0;
            this.mYearsInterval = 0;
            this.mCount = 0;
            this.mPatternInitialized = false;
            this.InternalName = name;
            log.Info("Created TimerTrigger \"" + name + "\".");
        }

        private long CalcNextTriggerTime(long ticks)
        {
            if (this.mCount == 0)
            {
                return -1;
            }

            this.mCount = this.mCount - 1;
            DateTime time1 = new DateTime(ticks);
            time1 = time1.AddSeconds(this.mSecondsInterval);
            time1 = time1.AddMinutes(this.mMinutesInterval);
            time1 = time1.AddHours(this.mHoursInterval);
            time1 = time1.AddDays(this.mDaysInterval);
            time1 = time1.AddMonths(this.mMonthsInterval);
            time1 = time1.AddYears(this.mYearsInterval);
            return time1.Ticks;
        }

        protected internal override void Fire(DateTime now)
        {
            lock (this.SynchObject)
            {
                if (this.InternalState == TriggerState.Expired)
                {
                    log.Error("Cannot fire a expired trigger. TimerTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Cannot fire a expired trigger.");
                }

                if (this.InternalState != TriggerState.Initialized)
                {
                    log.Error("Cannot fire a uninitialized trigger. TimerTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Cannot fire a uninitialized trigger.");
                }

                this.LastTriggerTime = now.Ticks;
                if (this.mCount == 0)
                {
                    this.InternalState = TriggerState.Expired;
                    this.NextTriggerTime = -1;
                }
                else
                {
                    this.NextTriggerTime = this.CalcNextTriggerTime(this.LastTriggerTime);
                }

                log.Info("Firing TimerTrigger \"" + this.InternalName + "\".");
                base.Fire(now);
            }
        }

        public int GetRemainingCount()
        {
            lock (this.SynchObject)
            {
                return this.mCount;
            }
        }

        protected internal override void Init(long ticks)
        {
            lock (this.SynchObject)
            {
                if (this.InternalState == TriggerState.Initialized)
                {
                    log.Error("Trigger already initialized. TimerTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Trigger already initialized.");
                }

                if (this.mCount == 0)
                {
                    log.Error("Trigger count has not been set. TimerTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Trigger count has not been set.");
                }

                if (!this.mPatternInitialized)
                {
                    log.Error("Time expression is not set. TimerTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Time expression not set for the trigger \"" + this.InternalName + "\"");
                }

                new DateTime(ticks);
                this.NextTriggerTime = this.CalcNextTriggerTime(ticks);
                this.InternalState = TriggerState.Initialized;
                log.Info("Initialized TimerTrigger \"" + this.InternalName + "\".");
            }
        }

        internal override void ParseXML(XmlNode node)
        {
            XmlNode node1 = node.SelectSingleNode("./TimeExpression");
            this.SetTimeExpression(node1.InnerText);
            node1 = node.SelectSingleNode("./Count");
            this.SetCount(int.Parse(node1.InnerText));
        }

        public void SetCount(int count)
        {
            lock (this.SynchObject)
            {
                if (count <= 0)
                {
                    log.Error("Count  must be greater than zero. TimerTrigger \"" + this.InternalName + "\".");
                    throw new ArgumentException("Count must be greater than zero.");
                }

                switch (this.InternalState)
                {
                    case TriggerState.Uninitialized:
                    case TriggerState.Error:
                        this.mCount = count;
                        goto Label_00A5;

                    case TriggerState.Expired:
                        break;

                    default:
                        goto Label_00A5;
                }

                log.Error("Cannot change count for the expired trigger. TimerTrigger \"" + this.InternalName + "\".");
                throw new InvalidOperationException("Cannot change count for the expired trigger.");
                Label_00A5:
                log.Debug("Set count to " + count + ". TimerTrigger \"" + this.InternalName + "\".");
            }
        }

        public void SetTimeExpression(string timeExpression)
        {
            lock (this.SynchObject)
            {
                if (timeExpression == null)
                {
                    log.Error("Time Expression cannot be null. TimerTrigger \"" + this.InternalName + "\".");
                    throw new ArgumentNullException("timeExpression", "Time Expression cannot be null.");
                }

                if (this.InternalState != TriggerState.Uninitialized)
                {
                    log.Info("You cannot change time expression after the timer has been initialized. TimerTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("You cannot change time expression after the timer has been initialized.");
                }

                Match match1 = new Regex(@"(((?<sec>(\d+))s){0,1})(((?<min>(\d+))m){0,1})(((?<hr>(\d+))h){0,1})(((?<day>(\d+))d){0,1})(((?<month>(\d+))M){0,1})(((?<year>(\d+))Y){0,1})", RegexOptions.Compiled).Match(timeExpression.Trim());
                if (!string.IsNullOrEmpty(match1.Groups["sec"].Value))
                {
                    this.mSecondsInterval = int.Parse(match1.Groups["sec"].Value);
                }

                if (!string.IsNullOrEmpty(match1.Groups["min"].Value))
                {
                    this.mMinutesInterval = int.Parse(match1.Groups["min"].Value);
                }

                if (!string.IsNullOrEmpty(match1.Groups["hr"].Value))
                {
                    this.mHoursInterval = int.Parse(match1.Groups["hr"].Value);
                }

                if (!string.IsNullOrEmpty(match1.Groups["day"].Value))
                {
                    this.mDaysInterval = int.Parse(match1.Groups["day"].Value);
                }

                if (!string.IsNullOrEmpty(match1.Groups["month"].Value))
                {
                    this.mMonthsInterval = int.Parse(match1.Groups["month"].Value);
                }

                if (!string.IsNullOrEmpty(match1.Groups["year"].Value))
                {
                    this.mYearsInterval = int.Parse(match1.Groups["year"].Value);
                }

                if (((((this.mSecondsInterval == 0) && (this.mMinutesInterval == 0)) && ((this.mHoursInterval == 0) && (this.mDaysInterval == 0))) && ((this.mMonthsInterval == 0) && (this.mYearsInterval == 0))) || (match1.Length != timeExpression.Trim().Length))
                {
                    log.Info("Invalid time expression \"" + timeExpression.Trim() + "\". TimerTrigger \"" + this.InternalName + "\".");
                    throw new ArgumentException("Invalid Time Expression \"" + timeExpression.Trim() + "\"");
                }

                this.mPatternInitialized = true;
                log.Debug("Set timer expression to " + timeExpression + ". TimerTrigger \"" + this.InternalName + "\".");
            }
        }

        private static readonly ILog log = LogManager.GetLogger("Scheduler");
        private volatile int mCount;
        private int mDaysInterval;
        private int mHoursInterval;
        private int mMinutesInterval;
        private int mMonthsInterval;
        private volatile bool mPatternInitialized;
        private int mSecondsInterval;
        private int mYearsInterval;
    }
}