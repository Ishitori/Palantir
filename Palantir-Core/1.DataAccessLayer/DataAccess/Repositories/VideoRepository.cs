namespace Ix.Palantir.DataAccess.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class VideoRepository : IVideoRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public VideoRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void Save(Video video)
        {
            if (!video.IsTransient())
            {
                return;
            }
            
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                video.Id = dataGateway.Connection.Query<int>(@"insert into video(vkgroupid, posteddate, vkid, year, month, week, day, hour, minute, second, title, description, duration) values (@VkGroupId, @PostedDate, @VkId, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @Title, @Description, @Duration) RETURNING id", video).First();
            } 
        }

        public void Update(Video video)
        {
            if (video == null)
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"update video set vkgroupid = @VkGroupId, posteddate = @PostedDate, vkid = @VkId, year = @Year, month = @Month, week = @Week, day = @Day, hour = @Hour, minute = @Minute, second = @Second, title = @Title, description = @Description, duration = @Duration, likescount = @LikesCount where id = @Id", video);
            }
        }

        public void SaveComment(VideoComment comment)
        {
            if (!comment.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                comment.Id = dataGateway.Connection.Query<int>(@"insert into videocomment(vkid, creatorid, posteddate, year, month, week, day, hour, minute, second, vkgroupid, vkvideoid) values (@VkId, @CreatorId, @PostedDate, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @VkGroupId, @VkVideoId) RETURNING id", comment).First();
            }
        }

        public Video GetVideo(int vkGroupId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Video>().SingleOrDefault(p => p.VkGroupId == vkGroupId && p.VkId == vkId);
            }
        }

        public VideoComment GetVideoCommentByVkGroupId(int vkGroupId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<VideoComment>().SingleOrDefault(vc => vc.VkGroupId == vkGroupId && vc.VkId == vkId);
            }
        }

        public IList<Video> GetVideosByVkGroupId(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Video>().Where(p => p.VkGroupId == vkGroupId).ToList();
            }
        }
    }
}