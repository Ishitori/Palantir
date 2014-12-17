namespace Ix.Palantir.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Palantir.DomainModel.Comparers;

    [Serializable]
    public class MemberSubscriptionCollection : IEntity, IVkEntity
    {
        public MemberSubscriptionCollection(int vkGroupId, long vkMemberId, IList<MemberSubscription> subscriptions = null)
        {
            this.VkGroupId = vkGroupId;
            this.VkId = vkMemberId.ToString();
            
            if (subscriptions != null)
            {
                var uniqueSubscriptions = subscriptions.Distinct(new MemberSubscriptionEqualityComparer()).ToList();
                this.Subscriptions = new List<MemberSubscription>(uniqueSubscriptions);
            }
            else
            {
                this.Subscriptions = new List<MemberSubscription>();
            }
        }

        public int Id { get; set; }
        public int VkGroupId { get; set; }
        public string VkId { get; set; }
        public IList<MemberSubscription> Subscriptions { get; private set; }
    }
}