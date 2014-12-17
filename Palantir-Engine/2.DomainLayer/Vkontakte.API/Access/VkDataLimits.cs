namespace Ix.Palantir.Vkontakte.API.Access
{
    public class VkDataLimits : IVkDataLimits
    {
        public int MemberSubscriptionLimits
        {
            get { return 200; }
        }
    }
}