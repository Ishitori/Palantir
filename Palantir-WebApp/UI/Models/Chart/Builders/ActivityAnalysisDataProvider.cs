namespace Ix.Palantir.UI.Models.Chart.Builders
{
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Metrics;

    public class ActivityAnalysisDataProvider
    {
        private readonly IMemberAdvancedSearchService searchService;

        public ActivityAnalysisDataProvider(IMemberAdvancedSearchService searchService)
        {
            this.searchService = searchService;
        }

        public PieChartData GetLikesRepostCommentPieChartData(int id, long filterCode)
        {
            LRCDiagramDataInfo diagramData = this.searchService.GetLikesRepostCommentDiagramData(id, filterCode);
            return this.LikesRepostCommentDiagramData(diagramData);
        }

        public PieChartData GetTypeOfContentPieChartData(int id, long filterCode)
        {
            TypeOfContentDataInfo diagramData = this.searchService.GetTypeOfContentDiagramData(id, filterCode);
            return this.GetTypeOfContentPieChartData(diagramData);
        }

        private PieChartData LikesRepostCommentDiagramData(LRCDiagramDataInfo data)
        {
            var pieChartData = new PieChartData();

            pieChartData.AddItem(new PieChartDataItem() { Label = "Комментарии", Value = data.Comments });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Лайки", Value = data.Likes });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Репосты", Value = data.Reposts });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Посты", Value = data.Posts });

            return pieChartData;
        }

        private PieChartData GetTypeOfContentPieChartData(TypeOfContentDataInfo data)
        {
            var pieChartData = new PieChartData();

            pieChartData.AddItem(new PieChartDataItem() { Label = "Тексты", Value = data.Text });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Фотографии", Value = data.Photo });
            pieChartData.AddItem(new PieChartDataItem() { Label = "Видео", Value = data.Video });

            return pieChartData;
        }
    }
}