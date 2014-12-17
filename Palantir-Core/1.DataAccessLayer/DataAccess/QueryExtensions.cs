namespace Ix.Palantir.DataAccess
{
    using System.Linq;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.Querying.Common.DataFilters;

    public static class QueryExtensions
    {
        /// <summary>
        /// Фильтровать данные.
        /// </summary>
        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, IQueryBuilder queryBuider, DataFilter filter) where TEntity : class
        {
            return queryBuider.BuildFilteredQuery(query, filter);
        }
    }
}