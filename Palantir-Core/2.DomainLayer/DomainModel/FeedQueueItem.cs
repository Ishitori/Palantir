namespace Ix.Palantir.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Queueing.API.Command;

    [Serializable]
    [XmlSerializerFormat]
    [DataContract(Name = "FeedQueueItem")]
    public class FeedQueueItem : CommandMessage
    {
        public override string CommandName
        {
            get { return "FeedQueueItem"; }
        }

        [DataMember]
        public virtual int VkGroupId { get; set; }
        [DataMember]
        public virtual QueueItemType QueueItemType { get; set; }
        [DataMember]
        public virtual DateTime CreationDate { get; set; }

        public override bool IsCorrupted()
        {
            return false;
        }

        public override void OnAfterDeserialization()
        {
        }

        public FeedQueueItem Copy()
        {
            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();

            FeedQueueItem itemCopy = new FeedQueueItem()
            {
                VkGroupId = this.VkGroupId,
                QueueItemType = this.QueueItemType,
                CreationDate = dateTimeHelper.GetDateTimeNow(),
            };

            return itemCopy;
        }

        public override IDictionary<string, string> GetProperties()
        {
            IDictionary<string, string> properties = base.GetProperties();
            properties.Add("IxDataFeedType", this.QueueItemType.ToString());
            properties.Add("IxVkGroupId", this.VkGroupId.ToString());
            return properties;
        }
    }
}