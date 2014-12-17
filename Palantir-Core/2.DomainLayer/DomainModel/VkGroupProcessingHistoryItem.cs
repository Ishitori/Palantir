namespace Ix.Palantir.DomainModel
{
    using System;

    public class VkGroupProcessingHistoryItem : IEntity
    {
        public virtual int Id { get; set; }
        public virtual int VkGroupId { get; set; }
        public virtual DataFeedType FeedType { get; set; }
        public virtual DateTime FetchingDate { get; set; }
        public virtual string FetchingServer { get; set; }
        public virtual string FetchingProcess { get; set; }
        public virtual DateTime ProcessingDate { get; set; }
        public virtual string ProcessingServer { get; set; }
        public virtual string ProcessingProcess { get; set; }
    }
}