namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Collections;
    using System.Xml;
    using Exceptions;
    using Logging;

    public class CronTrigger : Trigger
    {
        private const string InvalidCronPatternMessage = "Invalid cron pattern.";
        private static readonly Hashtable gDaysOfWeekMapping = new Hashtable
                                                                   {
                                                                           { DayOfWeek.Sunday, 0 },
                                                                           { DayOfWeek.Monday, 1 },
                                                                           { DayOfWeek.Tuesday, 2 },
                                                                           { DayOfWeek.Wednesday, 3 },
                                                                           { DayOfWeek.Thursday, 4 },
                                                                           { DayOfWeek.Friday, 5 },
                                                                           { DayOfWeek.Saturday, 6 }
                                                                   };
        private static readonly ILog log = LogManager.GetLogger("Scheduler");
        private static readonly int[] maxes = new[] { 0x3b, 0x3b, 0x17, 0x1f, 12, 6, 0xbb8 };
        private static readonly int[] mins = new[] { 0, 0, 0, 1, 1, 0, 0 };
        private readonly SortedList mDaysOfMonth;
        private readonly SortedList mDaysOfWeek;
        private readonly SortedList mHours;
        private readonly SortedList mMinutes;
        private readonly SortedList mMonths;
        private readonly SortedList mSeconds;
        private bool mAnyDayOfMonth;
        private bool mAnyDayOfWeek;
        private volatile bool mPatternInitialized;
        private SortedList mYears;

        public CronTrigger(string name)
        {
            this.mSeconds = new SortedList(60);
            this.mMinutes = new SortedList(60);
            this.mHours = new SortedList(0x18);
            this.mDaysOfWeek = new SortedList(7);
            this.mMonths = new SortedList(12);
            this.mDaysOfMonth = new SortedList(0x1f);
            this.mYears = new SortedList();
            this.mAnyDayOfWeek = false;
            this.mAnyDayOfMonth = false;
            this.mPatternInitialized = false;
            this.InternalName = name;

            log.Info("Created CronTrigger \"" + this.InternalName + "\".");
        }

        public long CalcNextTriggerTime(long ticks)
        {
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7;
            var time1 = new DateTime(ticks);
            time1 = time1.AddSeconds(1);
            num2 = time1.Second;
            int num8 = this.FindNext(this.mSeconds, num2);
            if (num8 == -1)
            {
                num8 = ((int)this.mSeconds.GetKey(0)) + 60;
            }
            else
            {
                num8 = (int)this.mSeconds.GetKey(num8);
            }

            time1 = time1.AddSeconds(num8 - num2);
            num2 = time1.Second;
            num3 = time1.Minute;
            num8 = this.FindNext(this.mMinutes, num3);
            if (num8 == -1)
            {
                time1 = time1.AddHours(1);
                num8 = (int)this.mMinutes.GetKey(0);
                num2 = (int)this.mSeconds.GetKey(0);
            }
            else
            {
                num8 = (int)this.mMinutes.GetKey(num8);
                if (num8 != num3)
                {
                    num2 = (int)this.mSeconds.GetKey(0);
                }
            }

            time1 = new DateTime(time1.Year, time1.Month, time1.Day, time1.Hour, num8, num2);
            num3 = time1.Minute;
            num4 = time1.Hour;
            num8 = this.FindNext(this.mHours, num4);
            if (num8 == -1)
            {
                time1 = time1.AddDays(1);
                num8 = (int)this.mHours.GetKey(0);
                num3 = (int)this.mMinutes.GetKey(0);
                num2 = (int)this.mSeconds.GetKey(0);
            }
            else
            {
                num8 = (int)this.mHours.GetKey(num8);
                if (num8 != num4)
                {
                    num3 = (int)this.mMinutes.GetKey(0);
                    num2 = (int)this.mSeconds.GetKey(0);
                }
            }

            time1 = new DateTime(time1.Year, time1.Month, time1.Day, num8, num3, num2);
            num4 = time1.Hour;
            if (!this.mAnyDayOfMonth && this.mAnyDayOfWeek)
            {
                num5 = time1.Day;
                num8 = this.FindNext(this.mDaysOfMonth, num5);
                if (num8 == -1)
                {
                    time1 = time1.AddMonths(1);
                    num8 = (int)this.mDaysOfMonth.GetKey(0);
                    num4 = (int)this.mHours.GetKey(0);
                    num3 = (int)this.mMinutes.GetKey(0);
                    num2 = (int)this.mSeconds.GetKey(0);
                }
                else
                {
                    num8 = (int)this.mDaysOfMonth.GetKey(num8);
                    if (num8 != num5)
                    {
                        num4 = (int)this.mHours.GetKey(0);
                        num3 = (int)this.mMinutes.GetKey(0);
                        num2 = (int)this.mSeconds.GetKey(0);
                    }
                }
            }
            else if (this.mAnyDayOfMonth && !this.mAnyDayOfWeek)
            {
                var num9 = (int)gDaysOfWeekMapping[time1.DayOfWeek];
                num8 = this.FindNext(this.mDaysOfWeek, num9);
                if (num8 == -1)
                {
                    num8 = ((int)this.mDaysOfWeek.GetKey(0)) + 7;
                    num4 = (int)this.mHours.GetKey(0);
                    num3 = (int)this.mMinutes.GetKey(0);
                    num2 = (int)this.mSeconds.GetKey(0);
                }
                else
                {
                    num8 = (int)this.mDaysOfWeek.GetKey(num8);
                    if (num8 != num9)
                    {
                        num4 = (int)this.mHours.GetKey(0);
                        num3 = (int)this.mMinutes.GetKey(0);
                        num2 = (int)this.mSeconds.GetKey(0);
                    }
                }

                time1 = time1.AddDays(num8 - num9);
                num8 = time1.Day;
            }
            else if (this.mAnyDayOfMonth && this.mAnyDayOfWeek)
            {
                num8 = time1.Day;
            }

            time1 = new DateTime(time1.Year, time1.Month, num8, num4, num3, num2);
            num5 = time1.Day;
            num6 = time1.Month;
            num8 = this.FindNext(this.mMonths, num6);
            if (num8 == -1)
            {
                time1 = time1.AddYears(1);
                num8 = (int)this.mMonths.GetKey(0);
                num4 = (int)this.mHours.GetKey(0);
                num3 = (int)this.mMinutes.GetKey(0);
                num2 = (int)this.mSeconds.GetKey(0);
                num5 = this.FindEarliestDay(time1.Year, num8);
            }
            else
            {
                num8 = (int)this.mMonths.GetKey(num8);
                if (num8 != num6)
                {
                    num5 = this.FindEarliestDay(time1.Year, num8);
                }
            }

            time1 = new DateTime(time1.Year, num8, num5, num4, num3, num2);
            num6 = time1.Month;
            if ((this.mYears != null) && (this.mYears.Count != 0))
            {
                num7 = time1.Year;
                num8 = this.FindNext(this.mYears, num7);
                if (num8 == -1)
                {
                    return -1;
                }

                num8 = (int)this.mYears.GetKey(num8);
                if (num8 != num7)
                {
                    num5 = this.FindEarliestDay(num8, num6);
                    time1 = new DateTime(num8, num6, num5, num4, num3, num2);
                }

                return time1.Ticks;
            }

            return time1.Ticks;
        }
        private int FindEarliestDay(int year, int month)
        {
            int num1 = -1;
            if (this.mAnyDayOfWeek && this.mAnyDayOfMonth)
            {
                return 1;
            }

            if (this.mAnyDayOfWeek && !this.mAnyDayOfMonth)
            {
                return (int)this.mDaysOfMonth.GetKey(0);
            }

            for (int num2 = 1; num2 < 8; num2++)
            {
                var time1 = new DateTime(year, month, num2);
                var num3 = (int)gDaysOfWeekMapping[time1.DayOfWeek];
                if (this.mDaysOfWeek.ContainsKey(num3))
                {
                    return num2;
                }
            }

            return num1;
        }
        private int FindNext(SortedList list, int element)
        {
            int num1 = -1;
            for (int num2 = list.Keys.Count - 1; num2 >= 0; num2--)
            {
                if (((int)list.GetKey(num2)) < element)
                {
                    return num1;
                }

                num1 = num2;
            }

            return num1;
        }

        protected internal override void Fire(DateTime now)
        {
            lock (this.SynchObject)
            {
                if (this.InternalState == TriggerState.Expired)
                {
                    log.Error("Cannot fire a expired trigger. CronTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Cannot fire a expired trigger.");
                }

                if (this.InternalState == TriggerState.Error)
                {
                    log.Error("Cannot fire a trigger with a state set to Error. Error message: \"" + this.InternalErrorMessage + "\". CronTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Cannot fire a trigger with a state set to Error.");
                }

                if (this.InternalState != TriggerState.Initialized)
                {
                    log.Error("Cannot fire a uninitialized trigger. CronTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Cannot fire a uninitialized trigger.");
                }

                this.LastTriggerTime = now.Ticks;
                try
                {
                    this.NextTriggerTime = this.CalcNextTriggerTime(this.LastTriggerTime);
                }
                catch (Exception exception1)
                {
                    if (ExceptionHelper.IsFatalException(exception1))
                    {
                        throw;
                    }

                    this.NextTriggerTime = -1;
                    this.InternalState = TriggerState.Error;
                    this.InternalErrorMessage = exception1.Message;
                }

                log.Info("Firing CronTrigger \"" + this.InternalName + "\".");
                base.Fire(now);
            }
        }

        private ArrayList GetStartEndListFromPattern(string pattern, int what)
        {
            var list1 = new ArrayList();
            if (pattern.IndexOf("/") == -1)
            {
                string[] textArray1 = pattern.Split(new[] { ',' });
                for (int num1 = 0; num1 < textArray1.Length; num1++)
                {
                    if (textArray1[num1].IndexOf('-') != -1)
                    {
                        string[] textArray2 = textArray1[num1].Split(new[] { '-' });
                        if (textArray2.Length != 2)
                        {
                            throw new ArgumentException(InvalidCronPatternMessage);
                        }

                        list1.Add(new object[] { int.Parse(textArray2[0]), int.Parse(textArray2[1]) });
                    }
                    else
                    {
                        list1.Add(new object[] { int.Parse(textArray1[num1]) });
                    }
                }
            }
            else
            {
                string dividerStr = pattern.Substring(pattern.IndexOf("/") + 1);
                int divider;
                if (string.IsNullOrEmpty(dividerStr) || !int.TryParse(dividerStr, out divider))
                {
                    throw new ArgumentException(InvalidCronPatternMessage);
                }

                int dividing = 0;
                switch (what)
                {
                    case 0:
                    case 1:
                        dividing = 59;
                        break;
                    case 2:
                        dividing = 23;
                        break;
                    default:
                        throw new ArgumentException(InvalidCronPatternMessage);
                }

                int times = dividing / divider;
                for (int i = 0; i <= dividing && times >= 0; i = i + divider, times--)
                {
                    list1.Add(new object[] { i });
                }
            }

            return list1;
        }

        protected internal override void Init(long ticks)
        {
            lock (this.SynchObject)
            {
                if (this.InternalState == TriggerState.Initialized)
                {
                    log.Error("Trigger already initialized. CronTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Trigger already initialized.");
                }

                if (!this.mPatternInitialized)
                {
                    log.Error("Crontab not set. CronTrigger \"" + this.InternalName + "\".");
                    throw new InvalidOperationException("Crontab not set for the trigger \"" + this.InternalName + "\"");
                }

                new DateTime(ticks);
                try
                {
                    this.NextTriggerTime = this.CalcNextTriggerTime(ticks);
                    this.InternalState = TriggerState.Initialized;
                }
                catch (Exception exception1)
                {
                    if (ExceptionHelper.IsFatalException(exception1))
                    {
                        throw;
                    }

                    this.NextTriggerTime = -1;
                    this.InternalState = TriggerState.Error;
                    this.InternalErrorMessage = exception1.Message;
                }

                log.Info("Initialized CronTrigger \"" + this.InternalName + "\".");
            }
        }

        private void ParsePattern(string pattern, int what)
        {
            bool flag1 = false;
            ArrayList list1 = null;
            if (pattern.IndexOf("*") != -1 && pattern.IndexOf("/") == -1)
            {
                flag1 = true;
            }
            else
            {
                list1 = this.GetStartEndListFromPattern(pattern, what);
            }

            SortedList list2;
            switch (what)
            {
                case 0:
                    list2 = this.mSeconds;
                    if (flag1)
                    {
                        int num1 = 0;
                        int num2 = 0x3b;
                        list1 = new ArrayList();
                        list1.Add(new object[] { num1, num2 });
                    }

                    break;

                case 1:
                    list2 = this.mMinutes;
                    if (flag1)
                    {
                        int num3 = 0;
                        int num4 = 0x3b;
                        list1 = new ArrayList();
                        list1.Add(new object[] { num3, num4 });
                    }

                    break;

                case 2:
                    list2 = this.mHours;
                    if (flag1)
                    {
                        int num5 = 0;
                        int num6 = 0x17;
                        list1 = new ArrayList();
                        list1.Add(new object[] { num5, num6 });
                    }

                    break;

                case 3:
                    list2 = this.mDaysOfMonth;
                    if (flag1)
                    {
                        this.mAnyDayOfMonth = true;
                        int num7 = 1;
                        int num8 = 0x1f;
                        list1 = new ArrayList();
                        list1.Add(new object[] { num7, num8 });
                    }

                    break;

                case 4:
                    list2 = this.mMonths;
                    if (flag1)
                    {
                        int num9 = 1;
                        int num10 = 12;
                        list1 = new ArrayList();
                        list1.Add(new object[] { num9, num10 });
                    }

                    break;

                case 5:
                    list2 = this.mDaysOfWeek;
                    if (flag1)
                    {
                        this.mAnyDayOfWeek = true;
                        int num11 = 0;
                        int num12 = 6;
                        list1 = new ArrayList();
                        list1.Add(new object[] { num11, num12 });
                    }

                    break;

                case 6:
                    if (flag1)
                    {
                        this.mYears = null;
                    }

                    list2 = this.mYears;
                    break;

                default:
                    throw new ArgumentException("Invalid cron pattern.");
            }

            if (what != 6)
            {
                foreach (object[] objArray1 in list1)
                {
                    if (objArray1.Length == 1)
                    {
                        list2[(int)objArray1[0]] = null;
                    }
                    else
                    {
                        int num13 = Math.Min((int)objArray1[0], (int)objArray1[1]);
                        int num14 = Math.Max((int)objArray1[0], (int)objArray1[1]);
                        for (int num15 = num13; num15 <= num14; num15++)
                        {
                            list2[num15] = null;
                        }
                    }
                }

                var num16 = (int)list2.GetKey(0);
                var num17 = (int)list2.GetKey(list2.Keys.Count - 1);
                if ((num16 < mins[what]) || (num17 > maxes[what]))
                {
                    switch (what)
                    {
                        case 0:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Seconds [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Seconds [", num17, "] Greater than allowable value of [", maxes[what], "]" }));

                        case 1:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Minutes [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Minutes [", num17, "] Greater than allowable value of [", maxes[what], "]" }));

                        case 2:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Hours [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Hours [", num17, "] Greater than allowable value of [", maxes[what], "]" }));

                        case 3:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Days of the Month [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Days of the Month [", num17, "] Greater than allowable value of [", maxes[what], "]" }));

                        case 4:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Months [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Months [", num17, "] Greater than allowable value of [", maxes[what], "]" }));

                        case 5:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Weekdays [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Weekdays [", num17, "] Greater than allowable value of [", maxes[what], "]" }));

                        case 6:
                            if (num16 < mins[what])
                            {
                                throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Min Value of Years [", num16, "] Less than allowable value of [", mins[what], "]" }));
                            }

                            throw new ArgumentException(string.Concat(new object[] { "Invalid cron pattern. Max Value of Years [", num17, "] Greater than allowable value of [", maxes[what], "]" }));
                    }
                }
            }
        }
        internal override void ParseXML(XmlNode node)
        {
            XmlNode node1 = node.SelectSingleNode("./Crontab/second");
            XmlNode node2 = node.SelectSingleNode("./Crontab/minute");
            XmlNode node3 = node.SelectSingleNode("./Crontab/hour");
            XmlNode node4 = node.SelectSingleNode("./Crontab/day");
            XmlNode node5 = node.SelectSingleNode("./Crontab/month");
            XmlNode node6 = node.SelectSingleNode("./Crontab/weekday");
            XmlNode node7 = node.SelectSingleNode("./Crontab/year");
            if ((((node1 == null) || (node2 == null)) || ((node3 == null) || (node4 == null))) || ((node5 == null) || (node6 == null)))
            {
                throw new ArgumentException("Invalid cron pattern.");
            }

            string text1 = node1.InnerText + " " + node2.InnerText + " " + node3.InnerText + " " + node4.InnerText + " " + node5.InnerText + " " + node6.InnerText;
            if (node7 != null)
            {
                text1 = text1 + " " + node7.InnerText;
            }

            this.SetCrontab(text1);
        }

        public void SetCrontab(string pattern)
        {
            lock (this.SynchObject)
            {
                string[] textArray1 = pattern.Split(new[] { ' ' });
                if ((textArray1.Length < 6) || (textArray1.Length > 7))
                {
                    log.Error("Invalid crontab: \"" + pattern + "\". CronTrigger \"" + this.InternalName + "\".");
                    throw new ArgumentException("Invalid crontab: \"" + pattern + "\".");
                }

                try
                {
                    for (int num1 = 0; num1 < textArray1.Length; num1++)
                    {
                        this.ParsePattern(textArray1[num1], num1);
                    }
                }
                catch (Exception exc)
                {
                    log.Error(exc.ToString());
                    throw new ArgumentException("Invalid crontab: \"" + pattern + "\". " + exc.Message + ". CronTrigger \"" + this.InternalName + "\".");
                }

                if (!this.mAnyDayOfWeek && !this.mAnyDayOfMonth)
                {
                    log.Error("Invalid crontab: \"" + pattern + "\". Specifying day(s) of the week and day(s) of the month in a same pattern is not supported.");
                    throw new ArgumentException("Invalid crontab: \"" + pattern + "\". Specifying day(s) of the week and day(s) of the month in a same pattern is not supported.");
                }

                this.mPatternInitialized = true;
                log.Debug("Crontab set to " + pattern + ". CronTrigger \"" + this.InternalName + "\".");
            }
        }
        public void SetCrontab(string second, string minute, string hour, string day, string month, string weekday)
        {
            this.SetCrontab(second + " " + minute + " " + hour + " " + day + " " + month + " " + weekday);
        }
    }
}