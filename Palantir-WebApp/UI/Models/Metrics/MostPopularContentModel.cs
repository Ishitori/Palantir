namespace Ix.Palantir.UI.Models.Metrics
{
    using System;
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Shared;

    /// <summary>
    /// Самый популярный контент.
    /// </summary>
    public class MostPopularContentModel
    {
        [UiTableColumn("", AutoNumeric = true)]
        public string Id { get; set; }

        [UiTableColumn("Опубликовано")]
        public DateTime PostedDate { get; set; }
        
        [UiTableColumn("Фото/видеоролик")]
        public UiLink Value { get; set; }

        [UiTableColumn("Тип")]
        public string Type { get; set; }

        [UiTableColumn("Комментарии", Sortable = SortableBy.BothDirection, ElementId = "commentCount")]
        public int CommentsCount { get; set; }

        [UiTableColumn("Лайки", Sortable = SortableBy.BothDirection, ElementId = "likesCount")]
        public int LikesCount { get; set; }

        [UiTableColumn("Репосты", Sortable = SortableBy.BothDirection, ElementId = "shareCount")]
        public int ShareCount { get; set; }

        [UiTableColumn("Сумма", Sortable = SortableBy.BothDirection, ElementId = "totalCount", DefaultSorted = SortableBy.Desc)]
        public int TotalCount { get; set; }
    }
}