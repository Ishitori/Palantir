namespace Ix.Palantir.UI.Models.Metrics
{
    using Ix.Palantir.UI.Attributes;
    using Ix.Palantir.UI.Models.Shared;

    public class MostActiveUsers
    {
        [UiTableColumn("", AutoNumeric = true)]
        public string RecordNo { get; set; }

        [UiTableColumn("Имя", Sortable = SortableBy.BothDirection, ElementId = "Name")]
        public UiLink Name { get; set; }

        [UiTableColumn("Постов", Sortable = SortableBy.BothDirection, ElementId = "PostCount")]
        public int NumberOfPosts { get; set; }

        [UiTableColumn("Комментариев", Sortable = SortableBy.BothDirection, ElementId = "СommentCount")]
        public int NumberOfComments { get; set; }

        [UiTableColumn("Лайков", Sortable = SortableBy.BothDirection, ElementId = "LikeCount")]
        public int NumberOfLikes { get; set; }

        [UiTableColumn("Репостов", Sortable = SortableBy.BothDirection, ElementId = "ShareCount")]
        public int NumberOfShare { get; set; }

        [UiTableColumn("Всего", Sortable = SortableBy.BothDirection, ElementId = "TotalCount", DefaultSorted = SortableBy.BothDirection)]
        public int Sum { get; set; }
    }
}