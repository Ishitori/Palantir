namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Dapper;
    using DomainModel;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.Logging;

    public class PhotoRepositoryCachingWrapper : IPhotoRepository
    {
        private readonly IPhotoRepository photosRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;
        private readonly ILog log;

        public PhotoRepositoryCachingWrapper(IPhotoRepository photosRepository, IDataGatewayProvider dataGatewayProvider, IFeedProcessingCachingStrategy cachingStrategy, ILog log)
        {
            this.photosRepository = photosRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.cachingStrategy = cachingStrategy;
            this.log = log;
        }

        public void Save(Photo photo)
        {
            try
            {
                this.photosRepository.Save(photo);
                this.cachingStrategy.StoreItem(photo, this.GetKey);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, photo, "photo");
            }
        }
        public void UpdatePhoto(Photo photo)
        {
            try
            {
                this.photosRepository.UpdatePhoto(photo);
                this.cachingStrategy.StoreItem(photo, this.GetKey);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, photo, "photo");
            }
        }

        public Photo GetPhotoByIdInAlbum(int vkGroupId, string vkAlbumId, string vkId)
        {
            var item = this.cachingStrategy.GetItem<Photo>(vkGroupId.ToString(), vkAlbumId, vkId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId, vkAlbumId), () => this.GetPhotos(vkGroupId, vkAlbumId), this.GetKey);
            return this.cachingStrategy.GetItem<Photo>(vkGroupId.ToString(), vkAlbumId, vkId);
        }

        public IList<string> GetGroupAlbumIds(int vkGroupId)
        {
            return this.photosRepository.GetGroupAlbumIds(vkGroupId);
        }

        private IEnumerable<IVkEntity> GetPhotos(int vkGroupId, string vkAlbumId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<Photo> photos = this.cachingStrategy.IsLimitedCachingEnabled(vkGroupId, DataFeedType.Photo)
                                         ? dataGateway.Connection.Query<Photo>("select * from photo where albumid = @albumid and posteddate > @postedDate", new { albumid = vkAlbumId, postedDate = this.cachingStrategy.GetDateLimit() })
                                         : dataGateway.Connection.Query<Photo>("select * from photo where albumid = @albumid", new { albumid = vkAlbumId });

                return photos;
            }
        }
        private string GetKey(IVkEntity entity)
        {
            Photo photo = (Photo)entity;
            var cacheId = this.cachingStrategy.GetCacheId(entity, photo.VkGroupId.ToString(), photo.AlbumId, photo.VkId);
            this.log.DebugFormat("Caching key generated for photo = {0}. VkGroupId = {1}, VkAlbumId = {2}, VkId = {3}", cacheId, photo.VkGroupId.ToString(), photo.AlbumId, photo.VkId);
            return cacheId;
        }

        private string GetInitCacheKey(int vkGroupId, string vkAlbumId)
        {
            return string.Format("VK_Photos_Processing_VkGroup_{0}_VkAlbum_{1}", vkGroupId, vkAlbumId);
        }
    }
}