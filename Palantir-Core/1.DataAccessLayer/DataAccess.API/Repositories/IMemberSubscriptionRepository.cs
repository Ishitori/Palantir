namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IMemberSubscriptionRepository
    {
        MemberSubscriptionCollection GetSubscriptions(int vkGroupId, long vkMemberId);
        void Save(MemberSubscription subscription);
        void Delete(MemberSubscription subscription);

        void Save(MemberSubscriptionCollection subscriptions, IList<MemberSubscription> existingSubscriptions = null);
        void Delete(MemberSubscriptionCollection subscriptions);
    }
}