namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IVkGroupRepository
    {
        VkGroup GetGroupById(int id);
        IList<VkGroup> GetGroups();

        DateTime? GetGroupCreationDate(int id);

        IList<Administrator> GetAdminstrators(int id);
        IList<long> GetAdministratorIds(int vkGroupId, bool appendGroupId = false);

        void SaveAdministator(Administrator admin);
        int SaveGroupProcessingHistoryItem(VkGroupProcessingHistoryItem item);

        IDictionary<DataFeedType, VkGroupProcessingState> GetLatestProcessingItems(int vkGroupId);
        VkGroupProcessingState GetProcessingState(int vkGroupId, DataFeedType feedType);
    }
}