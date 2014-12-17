namespace Ix.Palantir.DomainModel.Comparers
{
    using System.Collections.Generic;

    public class MemberSubscriptionEqualityComparer : IEqualityComparer<MemberSubscription>
    {
        public bool Equals(MemberSubscription x, MemberSubscription y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.SubscribedVkGroupId == y.SubscribedVkGroupId;
        }

        public int GetHashCode(MemberSubscription obj)
        {
            return obj.SubscribedVkGroupId;
        }
    }
}