namespace Ix.Palantir.Services.API
{
    /// <summary>
    /// Информация о посте.
    /// </summary>
    public class PostEntityInfo : EntityInfo
    {
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int LikesAndCommentsAndShareCount { get; set; }
        public int ShareCount { get; set; }
    }
}
