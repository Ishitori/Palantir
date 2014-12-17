namespace Ix.Palantir.DataAccess.API.Repositories
{
    using Ix.Palantir.DataAccess.API.Repositories.Changes;
    using Ix.Palantir.DomainModel;

    public interface IMemberRepository
    {
        void SaveMembersCount(MembersMetaInfo membersMeta);
        Member GetMember(int groupId, string memberId);
        void SaveMember(Member member);
        void UpdateMember(Member member, MemberChangeContext changeContext = null);
    }
}