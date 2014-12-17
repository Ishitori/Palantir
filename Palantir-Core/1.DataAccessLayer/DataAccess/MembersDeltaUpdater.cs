namespace Ix.Palantir.DataAccess
{
    using System;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;

    public class MembersDeltaUpdater : IMembersDeltaUpdater
    {
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly ILog log;

        public MembersDeltaUpdater(IDataGatewayProvider dataGatewayProvider, ILog log)
        {
            this.dataGatewayProvider = dataGatewayProvider;
            this.log = log;
        }

        public void CalculateMembersDelta(int vkGroupId, DateTime date)
        {
            this.log.DebugFormat("Start processing: vkGroupId = {0}", vkGroupId);

            var membersDelta = new MembersDelta
            {
                VkGroupId = vkGroupId,
                PostedDate = date
            };

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var newMembers = dataGateway.Connection.Query<long>("SELECT vkmemberid FROM memberupdate WHERE vkgroupid = @vkGroupId AND isnew = @isNew", new { vkGroupId,  isNew = true }).ToList();
                var deletedMembers = dataGateway.Connection.Query<long>("SELECT m.vkmemberid FROM member m LEFT OUTER JOIN memberupdate mu ON (m.vkgroupid = mu.vkgroupid AND m.vkmemberid = mu.vkmemberid) WHERE m.vkgroupid = @vkGroupId AND m.isdeleted = @isDeleted AND mu.id IS NULL", new { vkGroupId, isDeleted = false }).ToList();

                var newMembersBuilder = new SeparatedStringBuilder();
                var deletedMembersBuilder = new SeparatedStringBuilder();

                foreach (var memberId in newMembers)
                {
                    newMembersBuilder.AppendWithSeparator(memberId.ToString());
                }

                foreach (var memberId in deletedMembers)
                {
                    deletedMembersBuilder.AppendWithSeparator(memberId.ToString());
                }

                membersDelta.InIds = newMembersBuilder.ToString();
                membersDelta.InCount = newMembers.Count();
                membersDelta.OutIds = deletedMembersBuilder.ToString();
                membersDelta.OutCount = deletedMembers.Count();

                dataGateway.Connection.Execute("INSERT INTO membersdelta (vkgroupid, posteddate, second, minute, hour, day, month, year, inids, incount, outids, outcount) VALUES (@VkGroupId, @PostedDate, @Second, @Minute, @Hour, @Day, @Month, @Year, @InIds, @InCount, @OutIds, @OutCount)", membersDelta);
                this.log.DebugFormat(
                    "Delta: InCount = {0}, OutCount = {1}, InIds = [{2}], OutIds = [{3}]", 
                    membersDelta.InCount, 
                    membersDelta.OutCount,
                    membersDelta.InIds,
                    membersDelta.OutIds);

                dataGateway.Connection.Execute("DELETE FROM memberupdate WHERE vkgroupid = @vkGroupId", new { vkGroupId });

                var markAsDeletedQuery = string.Format("UPDATE member SET isdeleted = true WHERE vkgroupid = @vkGroupId AND vkmemberid = ANY ({0})", QueryArrayBuilder.GetString(deletedMembers.ToArray()));
                dataGateway.Connection.Execute(markAsDeletedQuery, new { vkGroupId });
            }
        }
    }
}
