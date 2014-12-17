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
    [DataContract(Name = "GroupQueueItem")]
    public class GroupQueueItem : CommandMessage
    {
        public GroupQueueItem(int vkGroupId)
        {
            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();
            this.VkGroupId = vkGroupId;
            this.CreationDate = dateTimeHelper.GetDateTimeNow();
        }

        [DataMember]
        public int VkGroupId { get; set; }
        [DataMember]
        public DateTime CreationDate { get; set; }

        public override string CommandName
        {
            get { return "GroupQueueItem"; }
        }
        public override bool IsCorrupted()
        {
            return false;
        }
        public override void OnAfterDeserialization()
        {
        }

        public override IDictionary<string, string> GetProperties()
        {
            IDictionary<string, string> properties = base.GetProperties();
            properties.Add("IxVkGroupId", this.VkGroupId.ToString());
            return properties;
        }
    }
}