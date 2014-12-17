namespace Ix.Palantir.DataAccess.API.StatisticsProviders
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;

    public interface IIrChartDataProvider
    {
        IEnumerable<IEnumerable<PointInTime>> GetInteractionRate(int vkGroupId, DateRange range, Periodicity periodicity);
    }
}