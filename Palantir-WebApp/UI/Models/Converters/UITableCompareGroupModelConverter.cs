namespace Ix.Palantir.UI.Models.Converters
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API.Analytics;
    using Ix.Palantir.UI.Formatters;
    using Ix.Palantir.UI.Models.Shared;

    public class UiTableCompareGroupModelConverter
    {
        private const string CONST_MoreIsBetterCssClass = "moreIsBetter";
        private const string CONST_LessIsBetterCssClass = "lessIsBetter";

        public IList<UiTableColumn> CreateCompareGroupsModel(ConcurrentAnalysisModel analysis)
        {
            IList<UiTableColumn> result = new List<UiTableColumn>();

            var valueColumn = GetLabelsColumn();
            result.Add(valueColumn);

            var targetColumn = this.GetColumn(analysis.BaseMetrics);
            result.Add(targetColumn);

            foreach (var comparativeModel in analysis.Comparisons)
            {
                var groupColumn = this.GetColumn(comparativeModel);
                result.Add(groupColumn);
            }

            return result;
        }

        private static UiTableColumn GetLabelsColumn()
        {
            var valueColumn = new UiTableColumn(string.Empty);

            valueColumn.AddItem(string.Empty);
            valueColumn.AddItem("Кол-во общих пользователей между целевой группой и данной*", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Среднее кол-во комментариев на пост", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Среднее кол-во лайков на пост", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Interaction rate (IR)", CONST_MoreIsBetterCssClass, true);
            valueColumn.AddItem("Response rate (RR)", CONST_MoreIsBetterCssClass, true);
            valueColumn.AddItem("Response Time (RT)", CONST_LessIsBetterCssClass, true);
            valueColumn.AddItem("Involvement rate", CONST_MoreIsBetterCssClass, true);
            valueColumn.AddItem("Ugc rate", CONST_MoreIsBetterCssClass, true);
            valueColumn.AddItem("Кол-во фото", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во видео", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во постов", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во тем", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Рассказать друзьям на пост", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во сообщений в темах", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во постов пользователей", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во постов администраторов", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Среднее кол-во постов на пользователя", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Среднее кол-во постов на администраторов", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Время наибольшей активности", CONST_MoreIsBetterCssClass, true);
            valueColumn.AddItem("Кол-во пользователей*", CONST_MoreIsBetterCssClass);
            valueColumn.AddItem("Кол-во блокированных или удаленных пользователей*", CONST_LessIsBetterCssClass);
            valueColumn.AddItem("Кол-во ботов*", CONST_LessIsBetterCssClass);
            valueColumn.AddItem("Кол-во неактивных пользователей", CONST_LessIsBetterCssClass);
            valueColumn.AddItem("Кол-во активных пользователей", CONST_MoreIsBetterCssClass);

            return valueColumn;
        }
        private UiTableColumn GetColumn(RankedModel metrics)
        {
            var targetColumn = new UiTableColumn(metrics.ProjectTitle);

            targetColumn.AddItem(metrics.ProjectTitle);
            targetColumn.AddItem(metrics.SameUsersCount);
            targetColumn.AddItem(metrics.AverageCommentsPerPost.Value, metrics.AverageCommentsPerPost.Rank);
            targetColumn.AddItem(metrics.AverageLikesPerPost.Value, metrics.AverageLikesPerPost.Rank);
            targetColumn.AddItem(metrics.InteractionRate.Value, metrics.InteractionRate.Rank);
            targetColumn.AddItem(metrics.ResponseRate.Value, metrics.ResponseRate.Rank);
            targetColumn.AddItem(metrics.ResponseTime.Value, metrics.ResponseTime.Rank);
            targetColumn.AddItem(metrics.InvolmentRate.Value, metrics.InvolmentRate.Rank);
            targetColumn.AddItem(metrics.UgcRate.Value, metrics.UgcRate.Rank);

            targetColumn.AddItem(metrics.PhotosCount.Value, metrics.PhotosCount.Rank);
            targetColumn.AddItem(metrics.VideosCount.Value, metrics.VideosCount.Rank);
            targetColumn.AddItem(metrics.PostsCount.Value, metrics.PostsCount.Rank);
            targetColumn.AddItem(metrics.TopicsCount.Value, metrics.TopicsCount.Rank);
            targetColumn.AddItem(metrics.SharePerPost.Value, metrics.SharePerPost.Rank);
            targetColumn.AddItem(metrics.TopicCommentsCount.Value, metrics.TopicCommentsCount.Rank);

            targetColumn.AddItem(metrics.UsersPostCount.Value, metrics.UsersPostCount.Rank);
            targetColumn.AddItem(metrics.AdminPostCount.Value, metrics.AdminPostCount.Rank);

            targetColumn.AddItem(metrics.UsersPostsPerUser.Value, metrics.UsersPostsPerUser.Rank);
            targetColumn.AddItem(metrics.AdminPostsPerAdmin.Value, metrics.AdminPostsPerAdmin.Rank);
            
            targetColumn.AddItem(new UiPostDensityFormatter(metrics.PostBiggestActivities));

            targetColumn.AddItem(metrics.UsersCount.Value, metrics.UsersCount.Rank);
            targetColumn.AddItem(metrics.BadFans.Value, metrics.BadFans.Rank);
            targetColumn.AddItem(metrics.BotsCount.Value, metrics.BotsCount.Rank);
            targetColumn.AddItem(metrics.InactiveUsersCount.Value, metrics.InteractionRate.Rank);
            targetColumn.AddItem(metrics.ActiveUsersCount.Value, metrics.ActiveUsersCount.Rank);

            return targetColumn;
        }
    }
}