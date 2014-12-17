namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.Caching;
    using Ix.Palantir.Configuration;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.LockServer.API;
    using Ix.Palantir.Logging;

    public class FeedProcessingCachingStrategy : IFeedProcessingCachingStrategy
    {
        private const string CONST_CompletedSuffix = "_completed";
        private readonly ICacheStorage cacheStorage;

        private readonly CachingHelper cachingHelper;
        private readonly CachingSettings cachingSettings;
        private readonly IConfigurationProvider configurationProvider;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IVkGroupRepository groupRepository;
        private readonly ILockServer lockServer;
        private readonly ILog log;

        public FeedProcessingCachingStrategy(ILockServer lockServer, CachingHelper cachingHelper, CachingSettings cachingSettings, IVkGroupRepository groupRepository, IConfigurationProvider configurationProvider, IDateTimeHelper dateTimeHelper, ICacheStorage cacheStorage)
        {
            this.cachingHelper = cachingHelper;
            this.cachingSettings = cachingSettings;
            this.groupRepository = groupRepository;
            this.configurationProvider = configurationProvider;
            this.dateTimeHelper = dateTimeHelper;
            this.cacheStorage = cacheStorage;
            this.lockServer = lockServer;
            this.log = LogManager.GetLogger();
        }

        public string GetCacheId(IVkEntity entity, string vkMajorId, params string[] vkMinorIds)
        {
            return this.cachingHelper.GetCacheId(entity, vkMajorId, vkMinorIds);
        }

        public T GetItem<T>(string vkMajorId, params string[] vkMinorIds) where T : IVkEntity
        {
            return this.cachingHelper.GetItem<T>(vkMajorId, vkMinorIds);
        }

        public T GetItem<T>(int vkGroupId, string vkId) where T : IVkEntity
        {
            return this.cachingHelper.GetItem<T>(vkGroupId, vkId);
        }

        public void StoreItem(IVkEntity entity, Func<IVkEntity, string> getKey = null)
        {
            this.cachingHelper.StoreItem(entity, this.GetNextExpirationDate(), getKey);
        }

        public void RemoveItem(IVkEntity entity, Func<IVkEntity, string> getKey = null)
        {
            this.cachingHelper.RemoveItem(entity, getKey);
        }

        public bool IsCacheAlreadyExists(string initKey)
        {
            var expirationDate = this.cacheStorage.GetItem<DateTime?>(this.GetInitFinishedKey(initKey));

            if (expirationDate.HasValue && expirationDate.Value > this.dateTimeHelper.GetDateTimeNow())
            {
                this.log.DebugFormat("InitCacheIfNeeded: existingExpirationDate is found cache initialization is not needed. InitKey: {0}, existingExpirationDate {1}, current UTC date {2}", initKey, expirationDate, this.dateTimeHelper.GetDateTimeNow());
                return true;
            }

            return false;
        }

        public void InitCacheIfNeeded(string initKey, Func<IEnumerable<IVkEntity>> dataProvider, Func<IVkEntity, string> getKey = null, int? expirationInMinutes = null)
        {
            this.log.DebugFormat("InitCacheIfNeeded: Execution started. InitKey: {0}", initKey);
            this.log.DebugFormat("InitCacheIfNeeded: Ckecking cache outside of lock. InitKey: {0}", initKey);

            if (this.IsCacheAlreadyExists(initKey))
            {
                return;
            }

            this.lockServer.LockSection(
            initKey, 
            () =>
            {
                this.log.DebugFormat("InitCacheIfNeeded: Checking cache inside of lock. InitKey: {0}", initKey);

                if (this.IsCacheAlreadyExists(initKey))
                {
                    return;
                }

                this.log.DebugFormat("InitCacheIfNeeded: existingExpirationDate is not found. InitKey: {0}", initKey);

                DateTime value = this.GetNextExpirationDate(expirationInMinutes);
                this.cachingHelper.InitCache(dataProvider, value, getKey);
                this.cacheStorage.PutItem(this.GetInitFinishedKey(initKey), value, value);

                this.log.DebugFormat("InitCacheIfNeeded: Completed key is stored into couchbase. InitKey: {0}, Value: {1}", initKey, value);
            });
        }

        public DateTime GetDateLimit()
        {
            int monthToFetch = this.configurationProvider.GetConfigurationSection<FeedProcessingConfig>().MonthToFetch;
            DateTime dateLimit = this.dateTimeHelper.GetDateNoTimeNow().AddMonths(-monthToFetch).AddDays(-1); // -1 just to ignore different time zones
            return dateLimit;
        }

        public bool IsLimitedCachingEnabled(int vkGroupId, DataFeedType dataFeedType)
        {
            if (this.cachingSettings.CachingMode != CachingMode.HonorFeedProcessingConfig)
            {
                return false;
            }

            var item = this.groupRepository.GetProcessingState(vkGroupId, dataFeedType);
            return item != null;
        }

        private DateTime GetNextExpirationDate(int? expirationInMinutes = null)
        {
            int ttlInMinutes = expirationInMinutes.HasValue 
                ? expirationInMinutes.Value
                : this.cachingSettings.TTLInMinutes;

            return this.dateTimeHelper.GetDateTimeNow().AddMinutes(ttlInMinutes);
        }

        private string GetInitFinishedKey(string initKey)
        {
            string initCompletedKey = initKey + CONST_CompletedSuffix;
            return initCompletedKey;
        }
    }
}