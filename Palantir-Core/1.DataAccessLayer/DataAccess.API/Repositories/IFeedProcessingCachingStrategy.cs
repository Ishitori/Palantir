namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IFeedProcessingCachingStrategy
    {
        DateTime GetDateLimit();
        void StoreItem(IVkEntity entity, Func<IVkEntity, string> getKey = null);
        void RemoveItem(IVkEntity entity, Func<IVkEntity, string> getKey = null);

        T GetItem<T>(string vkMajorId, params string[] vkMinorIds) where T : IVkEntity;
        T GetItem<T>(int vkGroupId, string vkId) where T : IVkEntity;

        void InitCacheIfNeeded(string initKey, Func<IEnumerable<IVkEntity>> dataProvider, Func<IVkEntity, string> getKey = null, int? expirationInMinutes = null);
        string GetCacheId(IVkEntity entity, string vkMajorId, params string[] vkMinorIds);

        bool IsLimitedCachingEnabled(int vkGroupId, DataFeedType dataFeedType);

        bool IsCacheAlreadyExists(string initKey);
    }
}