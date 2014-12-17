namespace Ix.Palantir.DataAccess.API
{
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;

    public interface IMemberAdvancedSearchCache
    {
        void SetToCache(int vkGroupId, AudienceFilteringResult result);
        AudienceFilteringResult GetFromCache(int vkGroupId, long code);
    }
}