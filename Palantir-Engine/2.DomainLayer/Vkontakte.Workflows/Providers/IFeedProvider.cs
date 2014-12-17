namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Vkontakte.API.Access;

    internal interface IFeedProvider
    {
        QueueItemType SupportedFeedType { get; }
        DataFeedType ProvidedDataType { get; }
        IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup);
    }
}