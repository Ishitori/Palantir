namespace Ix.Palantir.DataAccess.API
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public interface IEntitiesForChartProvider
    {
        IEnumerable<PointInTime> GetEntitiesForChart<T>(IEnumerable<T> query, DateRange range, Periodicity periodicity, Func<IEnumerable<T>, double> getValueFunc) where T : DateEntity;
    }
}