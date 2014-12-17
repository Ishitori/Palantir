namespace Ix.Palantir.DomainModel
{
    public enum DataFeedType : int
    {
        Undefined = 0,
        WallPosts = 1,
        Photo = 2,
        Video = 4,
        Administrators = 5,
        WallPostComments = 6,
        Topic = 7,
        TopicComment = 8,
        MembersCount = 30,
        MembersInfo = 31,
        MemberLikes = 32,
        MemberShares = 33,
        MemberSubscription = 34,
        PhotoAlbumDetails = 40,
        VideoComments = 44,
        VideoLikes = 45
    }
}