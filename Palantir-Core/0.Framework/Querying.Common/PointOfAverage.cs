namespace Ix.Palantir.Querying.Common
{
    using System;

    public class PointOfAverage
    {
        public DateTime Date { get; set; }
        public long Value { get; set; }
        public int Count { get; set; }

        public int Average
        {
            get
            {
                if (this.Count == 0)
                {
                    return 0;
                }

                return (int)(this.Value / this.Count);
            }
        }

        public PointInTime GetPointInTime()
        {
            return new PointInTime()
            {
                Date = this.Date,
                Value = this.Average
            };
        }
    }
}