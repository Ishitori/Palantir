namespace Ix.Palantir.UI.Models.Chart.Builders.Trend
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Models.Converters;

    public abstract class TrendMultiChartBuilder : TrendChartBuilderBase
    {
        protected TrendMultiChartBuilder(IMetricsService metricsService)
        {
            this.MetricsService = metricsService;
        }

        protected IMetricsService MetricsService { get; private set; }

        public IEnumerable<TrendChartData> GetCharts(int projectId, FilteringPeriod period)
        {
            DateRange dateRange = DateRangeConverter.GetDateRange(period);
            dateRange.IsSpecified = false;
            Periodicity periodicity = this.GetPeriodicity(period);
            IList<TrendChartData> data;
            DateRange timeLimit = this.GetLimit(projectId);

            if (this.SetLimit(dateRange, timeLimit) || (timeLimit != null))
            {
                data = this.GetPoints(projectId, dateRange, periodicity).Select(x => new TrendChartData(x, period)).ToList();
                
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Limited = true;
                    data[i].MaxTimeLimit = timeLimit.To.ToString();
                    data[i].MinTimeLimit = timeLimit.From.ToString();
                }
            }
            else
            {
                data = this.GetPoints(projectId, dateRange, periodicity).Select(x => new TrendChartData(x, period)).ToList();
            }

            return data;
        }

        public IEnumerable<TrendChartData> GetCharts(int projectId, DateRange dateRange)
        {
            bool isOneDayRange = dateRange.DaysInRange <= 1;
            Periodicity periodicity = isOneDayRange ? Periodicity.ByHour : Periodicity.ByDay;     
            FilteringPeriod filteringPeriod = isOneDayRange ? FilteringPeriod.Day : FilteringPeriod.Month;
     
            IList<TrendChartData> data;
            DateRange timeLimit = this.GetLimit(projectId);
            
            if (this.SetLimit(dateRange, timeLimit))
            {
                data = this.GetPoints(projectId, dateRange, periodicity).Select(x => new TrendChartData(x, filteringPeriod)).ToList();
                
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Limited = true;
                    data[i].MaxTimeLimit = timeLimit.To.ToString();
                    data[i].MinTimeLimit = timeLimit.From.ToString();
                }
            }
            else
            {
                data = this.GetPoints(projectId, dateRange, periodicity).Select(x => new TrendChartData(x, filteringPeriod)).ToList();
            }
            return data;
        }

        protected abstract IEnumerable<IEnumerable<PointInTime>> GetPoints(int projectId, DateRange dateRange, Periodicity periodicity);

        protected virtual DateRange GetLimit(int projectId)
        {
            return null;
        }

        protected virtual bool SetLimit(DateRange range, DateRange limit)
        {
            return false;
        }
    }
}