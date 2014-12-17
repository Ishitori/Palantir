namespace Ix.Palantir.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;

    public class ListRepository : IListRepository
    {
        private const string CONST_DateLimitQueryPary = " and p.posteddate > @dateLimit";
        private readonly IDataGatewayProvider dataGatewayProvider;

        public ListRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public IList<string> GetPostVkIds(int vkGroupId, DateTime? dateLimit)
        {
            const string Query = @"select vkid from post p where vkgroupid = @vkGroupId";
            return this.GetVkIds(Query, vkGroupId, dateLimit);
        }

        public IList<string> GetPostCommentVkIds(int vkGroupId, DateTime? dateLimit)
        {
            const string Query = @"select pc.vkid from postcomment pc inner join post p ON (pc.vkpostid = p.vkid) where p.vkgroupid = @vkGroupId";
            return this.GetVkIds(Query, vkGroupId, dateLimit);
        }

        public IList<string> GetPhotoVkIds(int vkGroupId, DateTime? dateLimit)
        {
            const string Query = @"select vkid from photo p where vkgroupid = @vkGroupId";
            return this.GetVkIds(Query, vkGroupId, dateLimit);
        }

        public IList<string> GetVideoVkIds(int vkGroupId, DateTime? dateLimit)
        {
            const string Query = @"select vkid from video p where vkgroupid = @vkGroupId";
            return this.GetVkIds(Query, vkGroupId, dateLimit);
        }

        public IList<long> GetMemberVkIds(int vkGroupId)
        {
            const string Query = @"select vkmemberid from member m where vkgroupid = @vkGroupId";
            
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var ids = dataGateway.Connection.Query<long>(Query, new { vkGroupId }).ToList();
                return ids;
            }
        }

        private IList<string> GetVkIds(string query, int vkGroupId, DateTime? dateLimit)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                query += dateLimit.HasValue ? CONST_DateLimitQueryPary : string.Empty;

                var ids = dataGateway.Connection.Query<string>(query, new { vkGroupId, dateLimit = dateLimit.HasValue ? dateLimit.Value : DateTime.MinValue }).ToList();
                return ids;
            }
        }
    }
}