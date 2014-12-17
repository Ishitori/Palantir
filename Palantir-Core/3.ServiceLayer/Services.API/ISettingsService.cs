namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;

    public interface ISettingsService
    {
        IList<GroupProcessingItem> GetProcessingHistory(int projectId);
        void DeleteProject(int project);
    }
}