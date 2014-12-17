namespace Ix.Palantir.Services.API
{
    using System;

    public class PostDensity
    {
        public int AbsoluteValue { get; set; }
        public double RelativeValue { get; set; }
        public int BeginHour { get; set; }
        public int EndHour { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}