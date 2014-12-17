namespace Ix.Palantir.Services
{
    using System.Linq;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.Querying.Common.DataFilters;

    /// <summary>
    /// Фабрика фильтра данных.
    /// </summary>
    public class QueryBuilder : IQueryBuilder
    {
        /// <summary>
        /// Создать запрос с фильтрацией.
        /// </summary>
        public IQueryable<TEntity> BuildFilteredQuery<TEntity>(IQueryable<TEntity> query, DataFilter filter) where TEntity : class
        {
            return query;
        }
    }
}