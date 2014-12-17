namespace Ix.Palantir.DataAccess.API.Repositories
{
    using Ix.Palantir.DomainModel;

    public interface IMemberLikeSharesRepository
    {
        void SaveLike(MemberLike like);
        MemberLike GetLike(int groupId, string memberId, int itemId, LikeShareType itemType);

        void SaveShare(MemberShare share);
        MemberShare GetShare(int groupId, string memberId, int itemId, LikeShareType itemType);
    }
}