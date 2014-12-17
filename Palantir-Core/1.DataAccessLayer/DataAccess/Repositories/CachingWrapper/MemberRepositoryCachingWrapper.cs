namespace Ix.Palantir.DataAccess.Repositories.CachingWrapper
{
    using System.Collections.Generic;
    using System.Data.Common;
    using DomainModel;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.Repositories.Changes;
    using Ix.Palantir.DataAccess.API.StatisticsProviders;

    public class MemberRepositoryCachingWrapper : IMemberRepository
    {
        private readonly IMemberRepository membersRepository;
        private readonly IRawDataProvider rawDataProvider;
        private readonly IFeedProcessingCachingStrategy cachingStrategy;

        public MemberRepositoryCachingWrapper(IMemberRepository membersRepository, IRawDataProvider rawDataProvider, IFeedProcessingCachingStrategy cachingStrategy)
        {
            this.rawDataProvider = rawDataProvider;
            this.cachingStrategy = cachingStrategy;
            this.membersRepository = membersRepository;
        }

        public void SaveMembersCount(MembersMetaInfo membersMeta)
        {
            this.membersRepository.SaveMembersCount(membersMeta);
        }
        public void SaveMember(Member member)
        {
            try
            {
                this.membersRepository.SaveMember(member);
                this.cachingStrategy.StoreItem(member);
            }
            catch (DbException exc)
            {
                ExceptionHandler.HandleSaveException(exc, member);
            }
        }
        public void UpdateMember(Member member, MemberChangeContext changeContext = null)
        {
            this.membersRepository.UpdateMember(member, changeContext);
            this.cachingStrategy.StoreItem(member);
        }

        public Member GetMember(int vkGroupId, string memberId)
        {
            var item = this.cachingStrategy.GetItem<Member>(vkGroupId, memberId);

            if (item != null)
            {
                return item;
            }

            this.cachingStrategy.InitCacheIfNeeded(this.GetInitCacheKey(vkGroupId), () => this.GetMembers(vkGroupId));
            return this.cachingStrategy.GetItem<Member>(vkGroupId, memberId);
        }

        private IEnumerable<IVkEntity> GetMembers(int vkGroupId)
        {
            return this.rawDataProvider.GetMembers(vkGroupId);
        }

        private string GetInitCacheKey(int vkGroupId)
        {
            return string.Format("VK_Member_Processing_VkGroup_{0}", vkGroupId);
        }
    }
}