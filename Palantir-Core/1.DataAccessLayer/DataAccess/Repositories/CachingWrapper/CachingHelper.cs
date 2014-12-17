namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System;
    using System.Collections.Generic;
    using Caching;
    using DomainModel;

    public class CachingHelper
    {
        private readonly ICacheStorage cacheStorage;
        private readonly IEntityIdBuilder idBuilder;

        public CachingHelper(ICacheStorage cacheStorage, IEntityIdBuilder idBuilder)
        {
            this.cacheStorage = cacheStorage;
            this.idBuilder = idBuilder;
        }

        public void InitCache(Func<IEnumerable<IVkEntity>> dataProvider, DateTime newExpirationDate, Func<IVkEntity, string> getKey = null)
        {
            IEnumerable<IVkEntity> data = dataProvider();
            this.StoreItems(data, newExpirationDate, getKey);
        }
        public void StoreItem<T>(T entity, DateTime expirationDate, Func<IVkEntity, string> getKey = null) where T : IVkEntity
        {
            this.cacheStorage.PutItem(getKey != null ? getKey(entity) : this.GetFullVkId(entity), entity, expirationDate);
        }
        public void StoreItems<T>(IEnumerable<T> entities, DateTime expirationDate, Func<IVkEntity, string> getKey = null) where T : IVkEntity
        {
            foreach (T entity in entities)
            {
                this.cacheStorage.PutItem(getKey != null ? getKey(entity) : this.GetFullVkId(entity), entity, expirationDate);
            }
        }
        public void RemoveItem(IVkEntity entity, Func<IVkEntity, string> getKey)
        {
            this.cacheStorage.RemoveItem(getKey != null ? getKey(entity) : this.GetFullVkId(entity));
        }
        public T GetItem<T>(string vkMajorId, params string[] vkMinorIds) where T : IVkEntity
        {
            string entityId = this.idBuilder.CreateEntityId(typeof(T).Name, vkMajorId, vkMinorIds);
            return this.cacheStorage.GetItem<T>(entityId);
        }
        public T GetItem<T>(int vkGroupId, string vkId) where T : IVkEntity
        {
            return this.GetItem<T>(vkGroupId.ToString(), vkId);
        }

        public string GetCacheId(IVkEntity entity, string vkMajorId, params string[] vkMinorIds)
        {
            var entityId = this.idBuilder.CreateEntityId(entity.GetType().Name, vkMajorId, vkMinorIds);
            return entityId;
        }

        private string GetFullVkId(IVkEntity entity)
        {
            var fullVkId = this.idBuilder.CreateEntityId(entity.GetType().Name, entity.VkGroupId.ToString(), entity.VkId);
            return fullVkId;
        }
    }
}