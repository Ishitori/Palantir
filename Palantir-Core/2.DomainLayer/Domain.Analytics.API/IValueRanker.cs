namespace Ix.Palantir.Domain.Analytics.API
{
    using System.Collections.Generic;
    using Ix.Palantir.Querying.Common;

    public interface IValueRanker
    {
        void RankValues(RankedValue<double> baseValue, IEnumerable<RankedValue<double>> otherValues, bool moreIsBetter = true);
        void RankValues(RankedValue<int> baseValue, IEnumerable<RankedValue<int>> otherValues, bool moreIsBetter = true);
    }
}