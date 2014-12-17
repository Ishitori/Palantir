namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class VideoRepositoryCachingWrapper : IVideoRepository
    {
        private readonly IVideoRepository videoRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;

        public VideoRepositoryCachingWrapper(IVideoRepository videoRepository, IDataGatewayProvider dataGatewayProvider, IFeedProcessingCachingStrategy cachingStrategy)
        {
            this.videoRepository = videoRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.cachingStrategy = cachingStrategy;
        }

        public void Save(Video video)
        {
            try
            {
                this.videoRepository.Save(video);
                this.cachingStrategy.StoreItem(video);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, video, "video");
            }
        }

        public void Update(Video video)
        {
            this.videoRepository.Update(video);
            this.cachingStrategy.StoreItem(video);
        }

        public void SaveComment(VideoComment comment)
        {
            try
            {
                this.videoRepository.SaveComment(comment);
                this.cachingStrategy.StoreItem(comment);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, comment, "topiccomment");
            }
        }

        public Video GetVideo(int vkGroupId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<Video>(vkGroupId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetCacheItems(vkGroupId));
            return this.cachingStrategy.GetItem<Video>(vkGroupId, vkId);
        }

        public VideoComment GetVideoCommentByVkGroupId(int vkGroupId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<VideoComment>(vkGroupId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetCacheItems(vkGroupId));
            return this.cachingStrategy.GetItem<VideoComment>(vkGroupId, vkId);
        }

        public IList<Video> GetVideosByVkGroupId(int vkGroupId)
        {
            return this.GetVideos(vkGroupId).Cast<Video>().ToList();
        }

        private IEnumerable<IVkEntity> GetVideos(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<Video> videos = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.Video)
                                         ? dataGateway.Connection.Query<Video>("select * from video where vkgroupid = @vkgroupid and posteddate > @postedDate", new { vkgroupid = vkGroupId, postedDate = this.cachingStrategy.GetDateLimit() }) 
                                         : dataGateway.Connection.Query<Video>("select * from video where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId });

                return videos;
            }
        }

        private IEnumerable<IVkEntity> GetVideoComments(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<VideoComment> videoComments = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.VideoComments)
                                         ? dataGateway.Connection.Query<VideoComment>("select vc.* from videocomment vc where vc.vkgroupid = @vkgroupid and vc.posteddate > @postedDate", new { vkgroupid = vkGroupId, postedDate = this.cachingStrategy.GetDateLimit() })
                                         : dataGateway.Connection.Query<VideoComment>("select * from videocomment where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId });

                return videoComments;
            }
        }

        private IEnumerable<IVkEntity> GetCacheItems(int vkGroupId)
        {
            var items = new List<IVkEntity>();

            items.AddRange(this.GetVideos(vkGroupId));
            items.AddRange(this.GetVideoComments(vkGroupId));

            return items;
        }

        private string GetInitCacheKey(int vkGroupId)
        {
            return string.Format("VK_Video_Processing_VkGroup_{0}", vkGroupId);
        }
    }
}