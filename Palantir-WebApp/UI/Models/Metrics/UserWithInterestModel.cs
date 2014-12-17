namespace Ix.Palantir.UI.Models.Metrics
{
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Shared;

    /// <summary>
    /// Модель представления пользователей, имеющих данный интерес.
    /// </summary>
    public class UserWithInterestModel
    {
        [UiTableColumn("", AutoNumeric = true)]
        public string Id { get; set; }

        [UiTableColumn("Пользователь", Sortable = SortableBy.BothDirection, DefaultSorted = SortableBy.Asc, ElementId = "userName")]
        public UiLink UserLink { get; set; }
    }
}