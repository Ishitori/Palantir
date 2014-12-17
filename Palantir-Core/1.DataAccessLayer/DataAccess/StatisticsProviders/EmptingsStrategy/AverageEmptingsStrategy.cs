namespace Ix.Palantir.DataAccess.StatisticsProviders.EmptingsStrategy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.DataAccess.API.StatisticsProviders;

    using Querying.Common;

    public class AverageEmptingsStrategy : IFillEmptingsStrategy
    {
        public int GetValue(IList<PointInTime> actualData, int positionInResult, bool itemInActualData)
        {
            if (actualData == null || !actualData.Any())
            {
                return 0;
            }

            if (!itemInActualData || positionInResult == 0 || positionInResult == actualData.Count - 1)
            {
                return this.GetAverageValue(actualData);
            }

            double leftValue = this.GetNeighborValue(actualData, positionInResult, x => x - 1);
            double rightValue = this.GetNeighborValue(actualData, positionInResult, x => x + 1);

            if (leftValue > 0 && rightValue > 0)
            {
                return Convert.ToInt32((leftValue + rightValue) / 2);
            }

            return this.GetAverageValue(actualData);
        }

        private double GetNeighborValue(IList<PointInTime> actualData, int positionInResult, Func<int, int> getNextIndex)
        {
            int index = getNextIndex(positionInResult);

            while (index >= 0 && index <= actualData.Count - 1)
            {
                if (actualData[index].Value > 0)
                {
                    return actualData[index].Value;
                }

                index = getNextIndex(index);
            }

            return 0;
        }

        private int GetAverageValue(IList<PointInTime> actualData)
        {
            return Convert.ToInt32(actualData.Where(r => r.Value != 0).Average(r => r.Value));
        }
    }
}