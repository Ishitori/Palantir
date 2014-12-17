namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using Ix.Palantir.DomainModel;

    public interface IProvidingStrategy
    {
        bool IsLimitedProcessingEnabled(int vkGroupId, DataFeedType dataFeedType);
        int GetMonthLimit();
    }
}