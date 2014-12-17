namespace Ix.Palantir.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public class EntitiesForChartProvider : IEntitiesForChartProvider
    {
        public IEnumerable<PointInTime> GetEntitiesForChart<T>(IEnumerable<T> query, DateRange range, Periodicity periodicity, Func<IEnumerable<T>, double> getValueFunc) where T : DateEntity
        {
            IList<PointInTime> result = new List<PointInTime>();
            var timeIncrease = this.GetTimeIncreaseFunction(periodicity);
            var executedQuery = query.ToList();

            for (var date = range.From; date < range.To; date = timeIncrease(date))
            {
                IEnumerable<T> entities = executedQuery.Where(x => (x.PostedDate >= date) && (x.PostedDate < timeIncrease(date))).ToList();

                result.Add(new PointInTime
                {
                    Date = date,
                    Value = getValueFunc(entities)
                });
            }

            return result;
        }
        
        private Func<DateTime, DateTime> GetTimeIncreaseFunction(Periodicity periodicity)
        {
            Func<DateTime, DateTime> increase;

            switch (periodicity)
            {
                case Periodicity.ByHour:
                    increase = time => time.AddHours(1);
                    break;

                case Periodicity.ByDayWithHour:
                    increase = time => time.AddDays(1);
                    break;

                case Periodicity.ByDay:
                    increase = time => time.AddDays(1);
                    break;

                case Periodicity.ByWeek:
                    increase = time => time.AddDays(7);
                    break;

                case Periodicity.ByMonth:
                    increase = time => time.AddMonths(1);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("periodicity");
            }
            return increase;
        }
    }
}