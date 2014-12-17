namespace Ix.Palantir.DataAccess
{
    using Ix.Palantir.Caching;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;

    public class MemberAdvancedSearchCache : IMemberAdvancedSearchCache
    {
        private readonly ICacheStorage cacheStorage;

        public MemberAdvancedSearchCache(ICacheStorage cacheStorage)
        {
            this.cacheStorage = cacheStorage;
        }

        public void SetToCache(int vkGroupId, AudienceFilteringResult result)
        {
            this.cacheStorage.PutItem(this.GetKey(vkGroupId, result.Code), result);
        }

        public AudienceFilteringResult GetFromCache(int vkGroupId, long code)
        {
            return this.cacheStorage.GetItem<AudienceFilteringResult>(this.GetKey(vkGroupId, code));
        }

        private string GetKey(int vkGroupId, long code)
        {
            return string.Format("MemberAdvancedSearchResult_{0}_{1}", vkGroupId, code);
        }
    }
}