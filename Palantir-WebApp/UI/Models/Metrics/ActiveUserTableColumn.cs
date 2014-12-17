namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ActiveUserTableColumn
    {
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string NumberOfPosts { get; set; }

        [DataMember]
        public string NumberOfComments { get; set; }

        [DataMember]
        public string NumberOfLikes { get; set; }

        [DataMember]
        public string Total { get; set; }
    }
}