namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using Ix.Palantir.DomainModel;

    public interface IFeedProcessor
    {
        void Process(DataFeed dataFeed, VkGroup group);
        void ProcessTerminator(int vkGroupId, int feedTypeVersion);
    }
}