namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;

    using Ix.Palantir.DomainModel;

    public interface IFeedRepository
    {
        void AddVkGroupFeedFetchingToQueue(int vkGroupId);
        void PutItemsToQueue(IEnumerable<FeedQueueItem> queueItems);
        void PutVkGroupToQueue(GroupQueueItem groupQueueItem);
    }
}