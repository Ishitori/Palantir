namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API.Responses;

    public class AdministratorsFeedProcessor : IFeedProcessor
    {
        private readonly ILog log;
        private readonly IVkGroupRepository groupRepository;

        public AdministratorsFeedProcessor(IVkGroupRepository groupRepository, ILog log)
        {
            this.log = log;
            this.groupRepository = groupRepository;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var response = new AdminsResponse(dataFeed.Feed);
            IList<long> adminIds = response.AdminIds;
            var savedAdministrator = this.groupRepository.GetAdminstrators(group.Id).ToDictionary(x => x.UserId);
            
            foreach (var adminId in adminIds)
            {
                if (!savedAdministrator.ContainsKey(adminId))
                {
                    Administrator admin = new Administrator()
                    {
                        UserId = adminId,
                        VkGroupId = group.Id
                    };

                    this.log.DebugFormat("Administrator with UserId={0} is not found in database. Saving", adminId);
                    this.groupRepository.SaveAdministator(admin);
                }
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }
    }
}