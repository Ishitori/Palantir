namespace Ix.Palantir.Services.API
{
    public class ContentEntityInfo : EntityInfo
    {
        public bool IsVideo { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int ShareCount { get; set; }
        public int LikesAndCommentsAndShareCount { get; set; }
    }
}