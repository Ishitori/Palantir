namespace Ix.Palantir.DomainModel.Extensions
{
    using System.Collections.Generic;

    public static class PostListExtension
    {
        public static void AddPosts(this IList<Post> posts, Project project, IEnumerable<string> vkIds)
        {
            foreach (var id in vkIds)
            {
                var post = new Post
                {
                    VkId = id,
                    VkGroupId = project.VkGroup.Id
                };
                
                posts.Add(post);
            }
        }
    }
}