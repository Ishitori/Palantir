namespace Ix.Palantir.Localization.API
{
    using System;

    public class TimeFrame
    {
        public TimeFrame()
        {
        }

        public TimeFrame(DayOfWeek dayOfWeek, int beginHour, int endHour)
        {
            this.DayOfWeek = dayOfWeek;
            this.BeginHour = beginHour;
            this.EndHour = endHour;
        }

        public DayOfWeek DayOfWeek { get; set; }
        public int BeginHour { get; set; }
        public int EndHour { get; set; }

        public static TimeFrame Create(DateTime start, DateTime end)
        {
            TimeFrame frame = new TimeFrame
            {
                DayOfWeek = start.DayOfWeek,
                BeginHour = start.Hour,
                EndHour = start.Hour < end.Hour ? end.Hour : end.Hour + 24
            };

            return frame;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((TimeFrame)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)this.DayOfWeek;
                hashCode = (hashCode * 397) ^ this.BeginHour;
                hashCode = (hashCode * 397) ^ this.EndHour;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}-{2}", this.DayOfWeek, this.BeginHour, this.EndHour);
        }

        protected bool Equals(TimeFrame other)
        {
            return this.DayOfWeek == other.DayOfWeek && this.BeginHour == other.BeginHour && this.EndHour == other.EndHour;
        }
    }
}