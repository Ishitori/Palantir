namespace Ix.Palantir.Domain.Analytics
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.Domain.Analytics.API;
    using Ix.Palantir.Querying.Common;

    public class ValueRanker : IValueRanker
    {
        public void RankValues(RankedValue<double> baseValue, IEnumerable<RankedValue<double>> otherValues, bool moreIsBetter)
        {
            IList<RankedValue<double>> ranks = new List<RankedValue<double>>(otherValues);
            ranks.Add(baseValue);

            ranks = moreIsBetter
                        ? ranks.OrderByDescending(r => r.Value).ToList()
                        : ranks.OrderBy(r => r.Value).ToList();

            if (ranks.All(o => o.Value == ranks[0].Value))
            {
                return;
            }

            int currentRank = 1;
            ranks[0].Rank = currentRank;

            for (int i = 1; i < ranks.Count; i++)
            {
                if (ranks[i].Value != ranks[i - 1].Value)
                {
                    currentRank++;
                }

                ranks[i].Rank = currentRank;
            }
        }

        public void RankValues(RankedValue<int> baseValue, IEnumerable<RankedValue<int>> otherValues, bool moreIsBetter)
        {
            IList<RankedValue<int>> ranks = new List<RankedValue<int>>(otherValues);
            ranks.Add(baseValue);

            ranks = moreIsBetter
                        ? ranks.OrderByDescending(r => r.Value).ToList()
                        : ranks.OrderBy(r => r.Value).ToList();

            if (ranks.All(o => o.Value == ranks[0].Value))
            {
                return;
            }

            int currentRank = 1;
            ranks[0].Rank = currentRank;

            for (int i = 1; i < ranks.Count; i++)
            {
                if (ranks[i].Value != ranks[i - 1].Value)
                {
                    currentRank++;
                }

                ranks[i].Rank = currentRank;
            }
        }
    }
}