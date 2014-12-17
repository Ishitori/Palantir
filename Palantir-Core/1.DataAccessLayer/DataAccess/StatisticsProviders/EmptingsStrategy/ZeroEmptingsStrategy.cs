namespace Ix.Palantir.DataAccess.StatisticsProviders.EmptingsStrategy
{
    using System.Collections.Generic;

    using Ix.Palantir.DataAccess.API.StatisticsProviders;

    using Querying.Common;

    public class ZeroEmptingsStrategy : IFillEmptingsStrategy
    {
        public int GetValue(IList<PointInTime> actualData, int positionInResult, bool itemInActualData)
        {
            return 0;
        }
    }
}