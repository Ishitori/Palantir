namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using Ix.Palantir.DomainModel;

    public interface IProcessorFactory
    {
        IFeedProcessor Create(DataFeedType feedType);
    }
}