namespace Ix.Palantir.UrlManagement.API
{
    public interface IVkUrlProvider
    {
        string GetPostUrl(string vkGroupId, string postId);
        string GetMemberProfileUrl(int vkMemberId);
        string GetPhotoUrl(string url, int vkGroupId, string vkId);
        string GetVideoUrl(string vkGroupUrl, int vkGroupId, string vkId);
    }
}