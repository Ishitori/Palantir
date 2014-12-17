namespace Ix.Palantir.Services.API
{
    using System.Linq;

    using Ix.Palantir.Querying.Common.DataFilters;

    /// <summary>
    /// Фабрика фильтра данных.
    /// </summary>
    public interface IQueryBuilder
    {
        /// <summary>
        /// Создать запрос с фильтрацией.
        /// </summary>
        IQueryable<TEntity> BuildFilteredQuery<TEntity>(IQueryable<TEntity> query, DataFilter filter) where TEntity : class;
    }
}
