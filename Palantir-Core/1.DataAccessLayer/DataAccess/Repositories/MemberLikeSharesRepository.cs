namespace Ix.Palantir.DataAccess.Repositories
{
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class MemberLikeSharesRepository : IMemberLikeSharesRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public MemberLikeSharesRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void SaveLike(MemberLike like)
        {
            if (!like.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                like.Id = dataGateway.Connection.Query<int>(@"insert into memberlike(vkmemberid, itemid, itemtype, vkgroupid) values (@VkMemberId, @ItemId, @ItemType, @VkGroupId) RETURNING id", like).First();
            }
        }
        public void SaveShare(MemberShare share)
        {
            if (!share.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                share.Id = dataGateway.Connection.Query<int>(@"insert into membershare(vkmemberid, itemid, itemtype, vkgroupid) values (@VkMemberId, @ItemId, @ItemType, @VkGroupId) RETURNING id", share).First();
            }
        }

        public MemberLike GetLike(int groupId, string memberId, int itemId, LikeShareType itemType)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var like = dataGateway.Connection.Query<MemberLike>(
                    "select * from memberlike where vkmemberid = @memberid and itemid = @entityid and itemtype = @entitytype",
                    new { memberid = int.Parse(memberId), entityid = itemId, itemtype = (int)itemType }).FirstOrDefault();

                return like;
            }
        }

        public MemberShare GetShare(int groupId, string memberId, int itemId, LikeShareType itemType)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var share = dataGateway.Connection.Query<MemberShare>(
                    "select * from membershare where vkmemberid = @memberid and itemid = @entityid and itemtype = @entitytype",
                    new { memberid = int.Parse(memberId), entityid = itemId, itemtype = (int)itemType }).FirstOrDefault();

                return share;
            }
        }
    }
}