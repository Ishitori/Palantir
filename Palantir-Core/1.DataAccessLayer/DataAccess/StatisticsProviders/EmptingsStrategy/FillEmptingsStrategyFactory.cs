namespace Ix.Palantir.DataAccess.StatisticsProviders.EmptingsStrategy
{
    using DomainModel;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;

    public static class FillEmptingsStrategyFactory
    {
        public static IFillEmptingsStrategy Create<T>()
        {
            if (typeof(T) == typeof(MembersMetaInfo))
            {
                return new AverageEmptingsStrategy();
            }

            return new ZeroEmptingsStrategy();
        }
    }
}