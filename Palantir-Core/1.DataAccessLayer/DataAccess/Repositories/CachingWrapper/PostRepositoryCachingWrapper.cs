namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class PostRepositoryCachingWrapper : IPostRepository
    {
        private readonly IPostRepository postRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;

        public PostRepositoryCachingWrapper(IPostRepository postRepository, IDataGatewayProvider dataGatewayProvider, IFeedProcessingCachingStrategy cachingStrategy)
        {
            this.postRepository = postRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.cachingStrategy = cachingStrategy;
        }

        public void Save(Post post)
        {
            try
            {
                this.postRepository.Save(post);
                this.cachingStrategy.StoreItem(post);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, post, "post");
            }
        }
        public void UpdatePost(Post post)
        {
            this.postRepository.UpdatePost(post);
            this.cachingStrategy.StoreItem(post);
        }

        public Post GetPost(int vkGroupId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<Post>(vkGroupId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetCacheItems(vkGroupId));
            return this.cachingStrategy.GetItem<Post>(vkGroupId, vkId);
        }
        public IList<Post> GetPostsByVkGroupId(int vkGroupId)
        {
            return this.postRepository.GetPostsByVkGroupId(vkGroupId);
        }

        public void SaveComment(PostComment comment)
        {
            try
            {
                this.postRepository.SaveComment(comment);
                this.cachingStrategy.StoreItem(comment);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, comment, "postcomment");
            }
        }
        public void UpdatePostComment(PostComment postComment)
        {
            this.postRepository.UpdatePostComment(postComment);
            this.cachingStrategy.StoreItem(postComment);
        }

        public PostComment GetPostComment(int vkGroupId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<PostComment>(vkGroupId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetCacheItems(vkGroupId));
            return this.cachingStrategy.GetItem<PostComment>(vkGroupId, vkId);
        }
        public IList<PostComment> GetPostCommentsByVkGroupId(int vkGroupId)
        {
            return this.GetPostComments(vkGroupId).Cast<PostComment>().ToList();
        }

        private IEnumerable<IVkEntity> GetPosts(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<Post> posts = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.WallPosts)
                                         ? dataGateway.Connection.Query<Post>("select * from post where vkgroupid = @vkgroupid and posteddate > @postedDate", new { vkgroupid = vkGroupId, postedDate = this.cachingStrategy.GetDateLimit() })
                                         : dataGateway.Connection.Query<Post>("select * from post where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId });

                return posts;
            }
        }
        private IEnumerable<IVkEntity> GetPostComments(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<PostComment> postComments = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.WallPostComments)
                                         ? dataGateway.Connection.Query<PostComment>("select pc.* from postcomment pc inner join post p ON (p.vkgroupid = pc.vkgroupid AND p.vkid = pc.vkpostid) where p.vkgroupid = @vkgroupid and p.posteddate > @postedDate", new { vkgroupid = vkGroupId, postedDate = this.cachingStrategy.GetDateLimit() })
                                         : dataGateway.Connection.Query<PostComment>("select * from postcomment where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId });

                return postComments;
            }
        }
        private IEnumerable<IVkEntity> GetCacheItems(int vkGroupId)
        {
            var items = new List<IVkEntity>();

            items.AddRange(this.GetPosts(vkGroupId));
            items.AddRange(this.GetPostComments(vkGroupId));

            return items;
        }

        private string GetInitCacheKey(int vkGroupId)
        {
            return string.Format("VK_Post_Processing_VkGroup_{0}", vkGroupId);
        }
    }
}