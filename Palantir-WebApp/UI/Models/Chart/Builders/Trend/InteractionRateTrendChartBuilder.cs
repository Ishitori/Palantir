namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Models.Chart.Builders.Trend;

    public class InteractionRateTrendChartBuilder : TrendMultiChartBuilder
    {
        public InteractionRateTrendChartBuilder(IMetricsService metricsService) : base(metricsService)
        {
        }

        protected override IEnumerable<IEnumerable<PointInTime>> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            return this.MetricsService.GetInteractionRate(projectId, dateRange, periodicity);
        }

        protected override DateRange GetLimit(int projectId)
        {
            var m = this.MetricsService.MemberDateLimit(projectId);
            var p = this.MetricsService.PostDateLimit(projectId);
            return new DateRange(m.From >= p.From ? m.From : p.From, m.To <= p.To ? m.To : p.To);
        }

        protected override bool SetLimit(DateRange range, DateRange limit)
        {
            bool limited = false;

            if (limit != null)
            {
                if (range.From < limit.From)
                {
                    range.From = limit.From;
                    limited = true;
                }
                ////if (limit.To < range.To)
                ////{
                ////    range.To = limit.To;
                ////    limited = true;
                ////}
            }

            return limited;
        }
    }
}