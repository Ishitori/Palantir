namespace Ix.Palantir.Vkontakte.API.Access
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Vkontakte.API.Responses;
    using Ix.Palantir.Vkontakte.API.Responses.Countries;
    using VkGroupType = Ix.Palantir.Vkontakte.API.VkGroupType;

    public interface IVkDataProvider
    {
        string GetUserProfile(long userId);
        Ix.Palantir.Vkontakte.API.Responses.MemberInformation.response GetUserProfiles(IEnumerable<string> userIds);

        Ix.Palantir.Vkontakte.API.Responses.MembershipProfile.response GetGroupMembershipStatus(long? userId = null);
        Ix.Palantir.Vkontakte.API.Responses.GroupMembers.response GetGroupMemberIds(string groupId, int offset);
        Ix.Palantir.Vkontakte.API.Responses.GroupDefinition.response GetGroup(string groupId);
        Ix.Palantir.Vkontakte.API.Responses.GroupTopicComments.response GetTopicComments(string groupId, string topicId, int offset);
        Ix.Palantir.Vkontakte.API.Responses.GroupTopics.response GetTopics(string groupId, int offset);
        Ix.Palantir.Vkontakte.API.Responses.NewsFeed.response GetNewsFeed(string groupName, DateTime? lowerBound = null, DateTime? upperBound = null);
        Vkontakte.API.Responses.WallPosts.response GetWallPosts(string userId, int offset);
        Vkontakte.API.Responses.Photos.response GetPhotos(string userId, int offset);
        Vkontakte.API.Responses.Videos.response GetVideos(string userId, int offset);
        Vkontakte.API.Responses.WallPostComments.response GetWallPostComments(string wallPostId, string userId, int offset);
        Ix.Palantir.Vkontakte.API.Responses.PhotoAlbumDetailsResponse.response GetPhotoAlbumDetails(string groupId, string vkAlbumId, int offset);
        Ix.Palantir.Vkontakte.API.Responses.VideoComments.response GetVideoComments(string groupId, string videoId, int offset);
        Responses.LikeShareFeed.response GetLikes(string groupId, string relatedObjectId, LikeShareType type, int offsetCounter);
        Responses.LikeShareFeed.response GetShares(string groupId, string relatedObjectId, LikeShareType type, int offsetCounter);

        AdminsResponse GetGroupAministrators(string groupId, VkGroupType groupType);
        response GetCountries();
        Vkontakte.API.Responses.Cities.response GetCities(int offset);
        string GetPostStats(string groupId, string postId, DateTime from, DateTime to);
        Ix.Palantir.Vkontakte.API.Responses.MemberSubscription.response GetMemberSubscriptions(string userId, int offsetCounter);
    }
}