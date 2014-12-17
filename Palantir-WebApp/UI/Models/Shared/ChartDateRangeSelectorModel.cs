namespace Ix.Palantir.UI.Models.Shared
{
    using System;

    public class ChartDateRangeSelectorModel
    {
        public ChartDateRangeSelectorModel()
        {
        }
        public ChartDateRangeSelectorModel(DateTime startDate, DateTime endDate, string chartContainerId, string containerId, string startDateId, string endDateId, string buttonId, string action)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ChartContainerId = chartContainerId;
            this.ContainerId = containerId;
            this.StartDateId = startDateId;
            this.EndDateId = endDateId;
            this.ButtonId = buttonId;
            this.Action = action;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ChartContainerId { get; set; }

        public string ContainerId { get; set; }
        public string StartDateId { get; set; }
        public string EndDateId { get; set; }
        public string ButtonId { get; set; }
        public string Action { get; set; }
    }
}