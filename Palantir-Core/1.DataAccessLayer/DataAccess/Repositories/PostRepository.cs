namespace Ix.Palantir.DataAccess.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class PostRepository : IPostRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public PostRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void Save(Post post)
        {
            if (!post.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                post.Id = dataGateway.Connection.Query<int>(@"insert into post(vkgroupid, posteddate, vkid, year, month, week, day, hour, minute, second, likescount, creatorid, text, commentscount) values (@VkGroupId, @PostedDate, @VkId, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @LikesCount, @CreatorId, @Text, @CommentsCount) RETURNING id", post).First();
            }
        }
        public void UpdatePost(Post post)
        {
            if (post == null)
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"update post set vkgroupid = @VkGroupId, posteddate = @PostedDate, vkid = @VkId, year = @Year, month = @Month, week = @Week, day = @Day, hour = @Hour, minute = @Minute, second = @Second, likescount = @LikesCount, creatorid = @CreatorId, text = @Text, commentscount = @CommentsCount where id = @Id", post);
            }
        }

        public Post GetPost(int vkGroupId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Post>().SingleOrDefault(p => p.VkGroupId == vkGroupId && p.VkId == vkId);
            }
        }
        public IList<Post> GetPostsByVkGroupId(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Post>().Where(p => p.VkGroupId == vkGroupId).ToList();
            }
        }

        public PostComment GetPostComment(int vkGroupId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<PostComment>().SingleOrDefault(p => p.VkGroupId == vkGroupId && p.VkId == vkId);
            }
        }
        public IList<PostComment> GetPostCommentsByVkGroupId(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<PostComment>().Where(p => p.VkGroupId == vkGroupId).ToList();
            }
        }

        public void SaveComment(PostComment comment)
        {
            if (!comment.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                comment.Id = dataGateway.Connection.Query<int>(@"insert into postcomment(vkid, creatorid, posteddate, year, month, week, day, hour, minute, second, vkgroupid, replytouserid, replytovkid, vkpostid) values (@VkId, @CreatorId, @PostedDate, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @VkGroupId, @ReplyToUserId, @ReplyToVkId, @VkPostId) RETURNING id", comment).First();
            }
        }
        public void UpdatePostComment(PostComment postComment)
        {
            if (postComment == null)
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"update postcomment set vkid = @VkId, creatorid = @CreatorId, posteddate = @PostedDate, year = @Year, month = @Month, week = @Week, day = @Day, hour = @Hour, minute = @Minute, second = @Second, vkgroupid = @VkGroupId, replytouserid = @ReplyToUserId, replytovkid = @ReplyToVkId, vkpostid = @VkPostId where id = @Id", postComment);
            }
        }
    }
}