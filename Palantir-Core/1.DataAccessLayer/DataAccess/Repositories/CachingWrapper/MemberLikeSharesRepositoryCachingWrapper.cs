namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System.Collections.Generic;
    using System.Data.Common;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;

    public class MemberLikeSharesRepositoryCachingWrapper : IMemberLikeSharesRepository
    {
        private readonly IMemberLikeSharesRepository memberLikeSharesRepository;
        private readonly IRawDataProvider rawDataProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;
        private readonly ILog log;

        public MemberLikeSharesRepositoryCachingWrapper(IMemberLikeSharesRepository memberLikeSharesRepository, IRawDataProvider rawDataProvider, IFeedProcessingCachingStrategy cachingStrategy, ILog log)
        {
            this.memberLikeSharesRepository = memberLikeSharesRepository;
            this.rawDataProvider = rawDataProvider;
            this.cachingStrategy = cachingStrategy;
            this.log = log;
        }

        public void SaveLike(MemberLike like)
        {
            try
            {
                this.memberLikeSharesRepository.SaveLike(like);
                this.cachingStrategy.StoreItem(like, this.GetLikeKey);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, like, "MemberLike");
            }
        }

        public void SaveShare(MemberShare share)
        {
            try
            {
                this.memberLikeSharesRepository.SaveShare(share);
                this.cachingStrategy.StoreItem(share, this.GetShareKey);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, share, "MemberShare");
            }
        }

        public MemberLike GetLike(int vkGroupId, string memberId, int itemId, LikeShareType itemType)
        {
            var item = this.cachingStrategy.GetItem<MemberLike>(vkGroupId.ToString(), memberId, itemType.ToString(), itemId.ToString());

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitLikeCacheKey(vkGroupId), () => this.GetMemberLikes(vkGroupId), this.GetLikeKey);
            return this.cachingStrategy.GetItem<MemberLike>(vkGroupId.ToString(), memberId, itemType.ToString(), itemId.ToString());
        }

        public MemberShare GetShare(int vkGroupId, string memberId, int itemId, LikeShareType itemType)
        {
            var item = this.cachingStrategy.GetItem<MemberShare>(vkGroupId.ToString(), memberId, itemType.ToString(), itemId.ToString());

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitSharesCacheKey(vkGroupId), () => this.GetMemberShares(vkGroupId), this.GetShareKey);
            return this.cachingStrategy.GetItem<MemberShare>(vkGroupId.ToString(), memberId, itemType.ToString(), itemId.ToString());
        }

        private string GetLikeKey(IVkEntity entity)
        {
            var memberLike = (MemberLike)entity;
            var cacheId = this.cachingStrategy.GetCacheId(entity, memberLike.VkGroupId.ToString(), memberLike.VkMemberId.ToString(), memberLike.ItemType.ToString(), memberLike.ItemId.ToString());
            this.log.DebugFormat("Caching key generated for memberlike = {0}. VkGroupId = {1}, VkMemberId = {2}, ItemType = {3}, ItemId = {4}", cacheId, memberLike.VkGroupId.ToString(), memberLike.VkMemberId.ToString(), memberLike.ItemType.ToString(), memberLike.ItemId.ToString());
            
            return cacheId;
        }

        private IEnumerable<IVkEntity> GetMemberLikes(int vkGroupId)
        {
            return this.rawDataProvider.GetMemberLikes(vkGroupId);
        }

        private string GetInitLikeCacheKey(int vkGroupId)
        {
            return string.Format("VK_MemberLikes_Processing_VkGroup_{0}", vkGroupId);
        }    

        private string GetShareKey(IVkEntity entity)
        {
            var memberShare = (MemberShare)entity;
            var cacheId = this.cachingStrategy.GetCacheId(entity, memberShare.VkGroupId.ToString(), memberShare.VkMemberId.ToString(), memberShare.ItemType.ToString(), memberShare.ItemId.ToString());
            this.log.DebugFormat("Caching key generated for membershare = {0}. VkGroupId = {1}, VkMemberId = {2}, ItemType = {3}, ItemId = {4}", cacheId, memberShare.VkGroupId.ToString(), memberShare.VkMemberId.ToString(), memberShare.ItemType.ToString(), memberShare.ItemId.ToString());
            
            return cacheId;
        }

        private IEnumerable<IVkEntity> GetMemberShares(int vkGroupId)
        {
            return this.rawDataProvider.GetMemberShares(vkGroupId);
        }

        private string GetInitSharesCacheKey(int vkGroupId)
        {
            return string.Format("VK_MemberShares_Processing_VkGroup_{0}", vkGroupId);
        }    
    }
}