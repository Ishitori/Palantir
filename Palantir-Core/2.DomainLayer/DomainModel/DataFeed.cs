namespace Ix.Palantir.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    using Ix.Palantir.Queueing.API.Command;

    [Serializable]
    [XmlSerializerFormat]
    [DataContract(Name = "DataFeed")]
    public class DataFeed : CommandMessage
    {
        public static readonly string VkGroupKey = "IxVkGroupId";
        public static readonly string DataFeedTypeKey = "IxDataFeedType";

        public override string CommandName
        {
            get { return "DataFeed"; }
        }

        [DataMember]
        public virtual int VkGroupId
        {
            get; set;
        }
        [DataMember]
        public virtual string RelatedObjectId
        {
            get; set;
        }
        [DataMember]
        public virtual DateTime ReceivedAt
        {
            get; set;
        }
        [DataMember]
        public virtual string Feed
        {
            get; set;
        }
        [DataMember]
        public virtual DataFeedType Type
        {
            get; set;
        }

        [DataMember]
        public virtual string FetchingServer
        {
            get; set;
        }

        [DataMember]
        public virtual string FetchingProcess
        {
            get; set;
        }

        [DataMember]
        public virtual bool IsSequenceTerminator
        {
            get; set;
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
            var properties = base.GetProperties();
            properties.Add(VkGroupKey, this.VkGroupId.ToString());
            properties.Add(DataFeedTypeKey, this.Type.ToString());

            return properties;
        }
    }
}