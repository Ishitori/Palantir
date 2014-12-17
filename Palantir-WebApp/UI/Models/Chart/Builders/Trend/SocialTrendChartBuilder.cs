namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Models.Chart.Builders.Trend;

    public class SocialTrendChartBuilder : TrendMultiChartBuilder
    {
        public SocialTrendChartBuilder(IMetricsService metricsService) : base(metricsService)
        {
        }

        protected override IEnumerable<IEnumerable<PointInTime>> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity)
        {
            // TODO: .SkipWhile(x, e => e.Value == 0))
            var result = this.MetricsService.GetUsersCount(projectId, dateRange, periodicity);
            return result;
        }

        protected override DateRange GetLimit(int projectId)
        {
            return this.MetricsService.MemberDateLimit(projectId);
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