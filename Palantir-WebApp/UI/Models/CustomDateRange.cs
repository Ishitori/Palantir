namespace Ix.Palantir.UI.Models
{
    using System;

    public class CustomDateRange
    {
        public CustomDateRange(DateTime startDate, DateTime endDate)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}