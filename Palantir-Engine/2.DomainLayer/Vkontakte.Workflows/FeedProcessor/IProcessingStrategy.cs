namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System;

    using Ix.Palantir.DomainModel;

    public interface IProcessingStrategy
    {
        bool IsLimitedProcessingEnabled(int vkGroupId, DataFeedType dataFeedType);
        int GetMonthLimit();

        DateTime? GetDateLimit(int vkGroupId, DataFeedType feedType);
    }
}