namespace Ix.Palantir.DomainModel
{
    using System;
    using Ix.Palantir.Utilities;

    [Serializable]
    public abstract class DateEntity
    {
        private DateTime postedDate;

        public virtual int Year
        {
            get;
            protected set;
        }
        public virtual int Month
        {
            get;
            protected set;
        }
        public virtual int Week
        {
            get;
            protected set;
        }
        public virtual int Day
        {
            get;
            protected set;
        }
        public virtual int Hour
        {
            get;
            protected set;
        }
        public virtual int Minute
        {
            get;
            protected set;
        }
        public virtual int Second
        {
            get;
            protected set;
        }

        public virtual DateTime PostedDate
        {
            get
            {
                return this.postedDate;
            }
            set
            {
                this.postedDate = value;
                this.Year = this.postedDate.Year;
                this.Month = this.postedDate.Month;
                this.Week = this.postedDate.GetWeekNumber();
                this.Day = this.postedDate.Day;
                this.Hour = this.postedDate.Hour;
                this.Minute = this.postedDate.Minute;
                this.Second = this.postedDate.Second;
            }
        }
    }
}
