namespace Ix.Palantir.DataAccess.StatisticsProviders
{
    public class InteractionRateCalculator
    {
        public double GetInteractionRate(int commentsCount, int likesCount, int postsCount, int sharecount, double membersCount)
         {
             double dividend = 100 * (likesCount + commentsCount + sharecount);
             return dividend / (postsCount * membersCount);
         }
    }
}