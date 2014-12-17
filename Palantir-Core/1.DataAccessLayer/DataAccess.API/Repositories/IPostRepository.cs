namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;

    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public interface IPostRepository
    {
        void Save(Post post);
        void UpdatePost(Post post);

        Post GetPost(int vkGroupId, string vkId);
        IList<Post> GetPostsByVkGroupId(int vkGroupId);
        PostComment GetPostComment(int vkGroupId, string vkId);
        IList<PostComment> GetPostCommentsByVkGroupId(int vkGroupId);
        void SaveComment(PostComment comment);
        void UpdatePostComment(PostComment postComment);
    }
}