namespace Ix.Palantir.DataAccess.API.StatisticsProviders
{
    using System.Collections.Generic;

    using Ix.Palantir.Querying.Common;

    public interface IFillEmptingsStrategy
    {
        int GetValue(IList<PointInTime> actualData, int positionInResult, bool itemInActualData);
    }
}