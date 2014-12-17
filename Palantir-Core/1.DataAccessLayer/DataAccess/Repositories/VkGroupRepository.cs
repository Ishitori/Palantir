namespace Ix.Palantir.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;

    public class VkGroupRepository : IVkGroupRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public VkGroupRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public VkGroup GetGroupById(int id)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<VkGroup>().SingleOrDefault(g => g.Id == id);
            }
        }
        public IList<VkGroup> GetGroups()
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<VkGroup>().ToList();
            }
        }

        public IList<Administrator> GetAdminstrators(int id)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<Administrator>().Where(a => a.VkGroupId == id).ToList();
            }
        }
        public IList<long> GetAdministratorIds(int vkGroupId, bool appendGroupId = false)
        {
            IList<long> adminIds;

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                adminIds = dataGateway.GetEntities<Administrator>().Where(x => x.VkGroupId == vkGroupId).Select(a => a.UserId).ToList();
            }

            if (appendGroupId)
            {
                adminIds.Add(-vkGroupId);
            }

            return adminIds;
        }
        public IDictionary<DataFeedType, VkGroupProcessingState> GetLatestProcessingItems(int vkGroupId)
        {
            IList<VkGroupProcessingState> items;
            IDictionary<DataFeedType, VkGroupProcessingState> uniqueItems = new Dictionary<DataFeedType, VkGroupProcessingState>();
            var possibleValues = Enum.GetValues(typeof(DataFeedType)).Cast<DataFeedType>().Where(t => t != DataFeedType.Undefined).ToList();

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                items = dataGateway.Connection.Query<VkGroupProcessingState>("select * from vkgroupprocessingstate where vkgroupid = @vkgroupid", new { vkgroupid = vkGroupId }).ToList();
            }

            foreach (var possibleValue in possibleValues)
            {
                var item = items.FirstOrDefault(i => i.FeedType == possibleValue);

                if (item != null)
                {
                    uniqueItems.Add(possibleValue, item);
                }
            }

            return uniqueItems;
        }
        public VkGroupProcessingState GetProcessingState(int vkGroupId, DataFeedType feedType)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var state = dataGateway.Connection.Query<VkGroupProcessingState>("select * from vkgroupprocessingstate where vkgroupid = @vkgroupid and feedtype = @feedType", new { vkgroupid = vkGroupId, feedType = (int)feedType }).FirstOrDefault();
                return state;
            }
        }

        public DateTime? GetGroupCreationDate(int id)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var firstPost = dataGateway.GetEntities<Post>().Where(p => p.VkGroupId == id).OrderBy(p => p.VkId).Take(1).SingleOrDefault();
                return firstPost == null ? (DateTime?)null : firstPost.PostedDate;
            }
        }

        public void SaveAdministator(Administrator admin)
        {
            if (!admin.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(admin);
            }
        }
        public int SaveGroupProcessingHistoryItem(VkGroupProcessingHistoryItem item)
        {
            if (!item.IsTransient())
            {
                return 0;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                item.Id = dataGateway.Connection.Query<int>(@"insert into vkgroupprocessinghistory(vkgroupid, feedtype, fetchingdate, fetchingserver, fetchingprocess, processingdate, processingserver, processingprocess) values (@VkGroupId, @FeedType, @FetchingDate, @FetchingServer, @FetchingProcess, @ProcessingDate, @ProcessingServer, @ProcessingProcess) RETURNING id", item).First();

                var state = dataGateway.Connection.Query<VkGroupProcessingState>("select * from vkgroupprocessingstate where vkgroupid = @vkGroupId and feedtype = @feedType", new { vkGroupId = item.VkGroupId, feedType = (int)item.FeedType }).FirstOrDefault() ?? new VkGroupProcessingState();
                state.FeedType = item.FeedType;
                state.VkGroupId = item.VkGroupId;
                state.FetchingDate = item.FetchingDate;
                state.FetchingProcess = item.FetchingProcess;
                state.FetchingServer = item.FetchingServer;
                state.ProcessingDate = item.ProcessingDate;
                state.ProcessingProcess = item.ProcessingProcess;
                state.ProcessingServer = item.ProcessingServer;
                state.Version++;

                if (state.IsTransient())
                {
                    item.Id = dataGateway.Connection.Query<int>(@"insert into vkgroupprocessingstate(version, vkgroupid, feedtype, fetchingdate, fetchingserver, fetchingprocess, processingdate, processingserver, processingprocess) values (@Version, @VkGroupId, @FeedType, @FetchingDate, @FetchingServer, @FetchingProcess, @ProcessingDate, @ProcessingServer, @ProcessingProcess) RETURNING id", state).First();
                }
                else
                {
                    dataGateway.Connection.Execute(@"update vkgroupprocessingstate set version = @Version, vkgroupid = @VkGroupId, feedtype = @FeedType, fetchingdate = @FetchingDate, fetchingserver = @FetchingServer, fetchingprocess = @FetchingProcess, processingdate = @ProcessingDate, processingserver = @ProcessingServer, processingprocess = @ProcessingProcess where id = @Id", state);
                }

                return state.Version;
            }
        }
    }
}