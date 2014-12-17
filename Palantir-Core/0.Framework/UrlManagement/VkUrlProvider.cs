namespace Ix.Palantir.UrlManagement
{
    using Ix.Palantir.UrlManagement.API;

    public class VkUrlProvider : IVkUrlProvider
    {
        public string GetPostUrl(string vkGroupId, string postId)
        {
            return string.Format("http://vk.com/wall-{0}_{1}", vkGroupId, postId);
        }
        public string GetMemberProfileUrl(int vkMemberId)
        {
            return string.Format("http://vk.com/id{0}", vkMemberId);
        }
        public string GetPhotoUrl(string vkGroupUrl, int vkGroupId, string vkId)
        {
            return string.Format("{0}?z=photo-{1}_{2}", vkGroupUrl, vkGroupId, vkId);
        }
        public string GetVideoUrl(string vkGroupUrl, int vkGroupId, string vkId)
        {
            return string.Format("{0}?z=video-{1}_{2}", vkGroupUrl, vkGroupId, vkId);
        }
    }
}