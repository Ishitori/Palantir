namespace Ix.Palantir.Vkontakte.Workflows.VkMappers
{
    using Ix.Palantir.DataAccess.API.Repositories.Changes;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Vkontakte.API.Responses.MemberInformation;

    public interface IMemberMapper
    {
        Member CreateMember(responseUser memberData, VkGroup group);
        MemberChangeContext UpdateMemberFields(Member member, responseUser memberData);
    }
}