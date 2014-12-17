namespace Ix.Palantir.Domain.Analytics.API
{
    using Ix.Palantir.DomainModel;

    public class MemberWithStatus
    {
        public long MemberId { get; set; }
        public MemberStatus Status { get; set; }
    }
}