namespace Ix.Palantir.Querying.Common.DataFilters
{
    /// <summary>
    /// Класс для филтрации данных.
    /// </summary>
    public class DataFilter
    {
        /// <summary>
        /// Выборка по датам.
        /// </summary>
        public DateRange DateRange { get; set; }

        /// <summary>
        /// Период выборки данных.
        /// </summary>
        public FilteringPeriod Period { get; set; }
    }
}
