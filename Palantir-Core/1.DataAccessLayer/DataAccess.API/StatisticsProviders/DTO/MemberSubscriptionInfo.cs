namespace Ix.Palantir.DataAccess.API.StatisticsProviders.DTO
{
    public class MemberSubscriptionInfo
    {
        public virtual int SubscribedVkGroupId { get; set; }
        public virtual string NameGroup { get; set; }
        public virtual string ScreenName { get; set; }
        public virtual string Photo { get; set; }
        public virtual int Count { get; set; }
    }
}