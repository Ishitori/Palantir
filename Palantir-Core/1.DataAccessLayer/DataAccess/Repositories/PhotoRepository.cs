namespace Ix.Palantir.DataAccess.Repositories
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Dapper;
    using DomainModel;

    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;

    public class PhotoRepository : IPhotoRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public PhotoRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void Save(Photo photo)
        {
            if (!photo.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                photo.Id = dataGateway.Connection.Query<int>(@"insert into photo(vkgroupid, albumid, posteddate, vkid, year, month, week, day, hour, minute, second, likescount, commentscount) values (@VkGroupId, @AlbumId, @PostedDate, @VkId, @Year, @Month, @Week, @Day, @Hour, @Minute, @Second, @LikesCount, @CommentsCount) RETURNING id", photo).First();
            }
        }
        public void UpdatePhoto(Photo savedPhoto)
        {
            Contract.Requires(savedPhoto != null);

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.Connection.Execute(@"update photo set vkgroupid = @VkGroupId, albumid = @AlbumId, posteddate = @PostedDate, vkid = @VkId, year = @Year, month = @Month, week = @Week, day = @Day, hour = @Hour, minute = @Minute, second = @Second, likescount = @LikesCount, commentscount = @CommentsCount where id = @Id", savedPhoto);
            }
        }

        public Photo GetPhotoByIdInAlbum(int vkGroupId, string vkAlbumId, string vkId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Photo>().SingleOrDefault(p => p.VkGroupId == vkGroupId && p.AlbumId == vkAlbumId && p.VkId == vkId);
            }
        }
        public IList<string> GetGroupAlbumIds(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.Connection.Query<string>("select albumid from photo where vkgroupid = @VkGroupId group by albumid", new { VkGroupId = vkGroupId }).ToList();
            }
        }
    }
}