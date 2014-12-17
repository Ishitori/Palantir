namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;

    public class MemberSubscriptionRepositoryCachingWrapper : IMemberSubscriptionRepository
    {
        private const int CONST_VkGroupReferenceTtlInMinutes = 2 * 60;

        private readonly IMemberSubscriptionRepository subscriptionRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;
        private readonly ILog log;

        public MemberSubscriptionRepositoryCachingWrapper(IMemberSubscriptionRepository subscriptionRepository, IDataGatewayProvider dataGatewayProvider, IFeedProcessingCachingStrategy cachingStrategy, ILog log)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.dataGatewayProvider = dataGatewayProvider;
            this.cachingStrategy = cachingStrategy;
            this.log = log;
        }

        public void Save(MemberSubscription subscription)
        {
            try
            {
                this.subscriptionRepository.Save(subscription);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, subscription, "subscription");
            }
        }

        public void Save(MemberSubscriptionCollection subscriptions, IList<MemberSubscription> existingSubscriptions)
        {
            try
            {
                this.subscriptionRepository.Save(subscriptions);

                if (existingSubscriptions != null)
                {
                    foreach (var existingSubscription in existingSubscriptions)
                    {
                        subscriptions.Subscriptions.Add(existingSubscription);
                    }
                }

                this.cachingStrategy.StoreItem(subscriptions, this.GetSubscriptionKey);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, subscriptions, "subscriptions");
            }
        }

        public void Delete(MemberSubscription subscription)
        {
            try
            {
                this.subscriptionRepository.Delete(subscription);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, subscription, "subscription");
            }
        }

        public void Delete(MemberSubscriptionCollection subscriptions)
        {
            try
            {
                this.subscriptionRepository.Delete(subscriptions);
                this.cachingStrategy.RemoveItem(subscriptions, this.GetSubscriptionKey);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, subscriptions, "subscriptions");
            }
        }

        public MemberSubscriptionCollection GetSubscriptions(int vkGroupId, long vkMemberId)
        {
            var item = this.cachingStrategy.GetItem<MemberSubscriptionCollection>(vkGroupId, vkMemberId.ToString());

            if (item != null)
            {
                return item;
            }

            this.InitCacheIfNeeded(vkGroupId);
            var collection = this.cachingStrategy.GetItem<MemberSubscriptionCollection>(vkGroupId, vkMemberId.ToString());
            return collection ?? new MemberSubscriptionCollection(vkGroupId, vkMemberId);
        }

        private IEnumerable<IVkEntity> GetSubscriptions(int vkGroupId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<MemberSubscription> subscriptions = dataGateway.Connection.Query<MemberSubscription, VkGroupReference, MemberSubscription>(
                    @"select ms.vkmemberid, ms.vkgroupid, ms.subscribedvkgroupid, vkr.vkgroupid, vkr.namegroup, vkr.screenname, vkr.photo from membersubscriptions ms inner join vkgroupreference vkr on (ms.subscribedvkgroupid = vkr.vkgroupid) where ms.vkgroupid = @VkGroupId",
                    (subscription, groupReference) =>
                    {
                        subscription.SubscribedVkGroup = groupReference;
                        return subscription;
                    },
                    new { vkGroupId },
                    splitOn: "vkr.vkgroupid").ToList();
                IDictionary<long, MemberSubscriptionCollection> collections = new Dictionary<long, MemberSubscriptionCollection>();

                foreach (var subscription in subscriptions)
                {
                    if (!collections.ContainsKey(subscription.VkMemberId))
                    {
                        var currentCollection = new MemberSubscriptionCollection(vkGroupId, subscription.VkMemberId);
                        currentCollection.Subscriptions.Add(subscription);
                        collections.Add(subscription.VkMemberId, currentCollection);
                    }
                    else
                    {
                        collections[subscription.VkMemberId].Subscriptions.Add(subscription);
                    }
                }

                return collections.Values;
            }
        }
        private IEnumerable<IVkEntity> GetVkGroupReferences()
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                IEnumerable<VkGroupReference> references = dataGateway.Connection.Query<VkGroupReference>(@"select vkr.vkgroupid, vkr.namegroup, vkr.screenname, vkr.photo from vkgroupreference vkr").ToList();
                return references;
            }
        }
        private string GetSubscriptionKey(IVkEntity entity)
        {
            var subscription = (MemberSubscriptionCollection)entity;
            var cacheId = this.cachingStrategy.GetCacheId(entity, subscription.VkGroupId.ToString(), subscription.VkId);
            this.log.DebugFormat("Caching key generated for membersubscription = {0}. VkGroupId = {1}, VkId = {2}", cacheId, subscription.VkGroupId.ToString(), subscription.VkId);
            return cacheId;
        }
        private string GetVkGroupReferenceKey(IVkEntity entity)
        {
            var groupReference = (VkGroupReference)entity;
            var cacheId = this.cachingStrategy.GetCacheId(entity, groupReference.VkGroupId.ToString(), groupReference.VkId);
            this.log.DebugFormat("Caching key generated for VkGroupReference = {0}. VkGroupId = {1}, VkId = {2}", cacheId, groupReference.VkGroupId.ToString(), groupReference.VkId);
            return cacheId;
        }

        private string GetInitSubscriptionCacheKey(int vkGroupId)
        {
            return string.Format("VK_MemberSubscriptionCollection_Processing_VkGroup_{0}", vkGroupId);
        }
        private string GetInitVkGroupReferenceCacheKey()
        {
            return "VK_VkGroupReference";
        }

        private void InitCacheIfNeeded(int vkGroupId)
        {
            var initCacheKey = this.GetInitSubscriptionCacheKey(vkGroupId);
            var isCacheAlreadyExists = this.cachingStrategy.IsCacheAlreadyExists(initCacheKey);

            if (isCacheAlreadyExists)
            {
                return;
            }

            this.cachingStrategy.InitCacheIfNeeded(initCacheKey, () => this.GetSubscriptions(vkGroupId), this.GetSubscriptionKey);
            this.cachingStrategy.InitCacheIfNeeded(this.GetInitVkGroupReferenceCacheKey(), this.GetVkGroupReferences, this.GetVkGroupReferenceKey, CONST_VkGroupReferenceTtlInMinutes);
        }
    }
}