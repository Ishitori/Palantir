namespace Ix.Palantir.UI.Models.Metrics
{
    using System.Web;
    using System.Web.Mvc;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Formatters;
    using Ix.Palantir.UI.Models.Shared;

    public class DashboardViewModel : MetricsViewModel
    {
        private readonly DashboardMetrics metrics;
        private readonly UrlHelper urlHelper;

        public DashboardViewModel(DashboardMetrics metrics) : base(metrics)
        {
            this.metrics = metrics;
            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();
            this.CustomDateRange = new CustomDateRange(dateTimeHelper.GetDateTimeNow().AddDays(-7), dateTimeHelper.GetDateTimeNow());
            this.urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            this.InitModels();
        }

        public virtual double UgcRate
        {
            get
            {
                return this.metrics.UgcRate;
            }
        }

        public string LastPostDate
        {
            get
            {
                return this.metrics.LastPostDate.HasValue ? this.metrics.LastPostDate.Value.ToString(string.Empty) : "-";
            }
        }

        public int UsersPostCount
        {
            get
            {
                return this.metrics.UsersPostCount;
            }
        }
        public int AdminPostCount
        {
            get
            {
                return this.metrics.AdminPostCount;
            }
        }

        public string UsersPostsPerUser
        {
            get
            {
                return this.metrics.UsersPostsPerUser != -1 ? this.metrics.UsersPostsPerUser.ToString() : "N/A";
            }
        }
        public string AdminPostsPerAdmin
        {
            get
            {
                return this.metrics.AdminPostsPerAdmin != -1 ? this.metrics.AdminPostsPerAdmin.ToString() : "N/A";
            }
        }

        public CustomDateRange CustomDateRange
        {
            get;
            set;
        }

        public KpiListModel Kpi { get; private set; }
        public KpiListModel Users { get; private set; }
        public KpiListModel Content { get; private set; }
        public KpiListModel Engagement { get; private set; }

        private void InitModels()
        {
            this.InitKpiModels();
            this.InitContentModel();
            this.InitUsersModel();
            this.InitEngagementModel();
        }
        private void InitKpiModels()
        {
            this.Kpi = new KpiListModel("KPI");
            this.Kpi.CssClass = "kpi-kpi";
            this.Kpi.AddItem("IR", this.metrics.InteractionRate, ValueType.Default, "tooltip ir-tooltip");
            this.Kpi.AddItem("RR", this.metrics.ResponseRate, ValueType.Default, "tooltip rr-tooltip");

            var formatter = new UIResponseTimeFromatter(this.metrics.ResponseTime);
            this.Kpi.AddItem("RT", formatter.ToString(), ValueType.Default, "tooltip rt-tooltip");
        }
        private void InitContentModel()
        {
            this.Content = new KpiListModel("Контент");
            this.Content.CssClass = "kpi-content";
            this.Content.DetailsUrl = this.urlHelper.Action("Index", "Content");

            this.Content.AddItem("Постов", this.metrics.PostsCount, ValueType.Default, "tooltip post-tooltip");
            this.Content.AddItem("Тем / сообщений", string.Format("{0} / {1}", this.metrics.TopicsCount, this.metrics.TopicCommentsCount), ValueType.Default, "tooltip tm-tooltip");
            this.Content.AddItem("Видео", this.metrics.VideosCount, ValueType.Default, "tooltip v-tooltip");
            this.Content.AddItem("Фотографии", this.metrics.PhotosCount, ValueType.Default, "tooltip ph-tooltip");
        }
        private void InitUsersModel()
        {
            this.Users = new KpiListModel("Пользователи");
            this.Users.CssClass = "kpi-users";
            this.Users.DetailsUrl = this.urlHelper.Action("Index", "Social");

            this.Users.AddItem("Активные", this.metrics.ActiveUsersCount, ValueType.Good, "tooltip au-tooltip");
            this.Users.AddItem("Неактивные", this.metrics.InactiveUsersCount, ValueType.Neutral, "tooltip nau-tooltip");
            this.Users.AddItem("Мертвые души", this.metrics.BadFans + this.metrics.BotsCount, ValueType.Bad, "tooltip du-tooltip");
            this.Users.AddItem("Всего", this.metrics.UsersCount, ValueType.Default, "tooltip t-tooltip");
        }
        private void InitEngagementModel()
        {
            this.Engagement = new KpiListModel("Вовлеченность");
            this.Engagement.CssClass = "kpi-engagement";

            var formatter = new UiPostDensityFormatter(this.metrics.PostBiggestActivities);
            string mostCrowdedTime = formatter.ToString();

            this.Engagement.AddItem("Комментариев на пост", this.metrics.AverageCommentsPerPost, ValueType.Neutral, "tooltip cp-tooltip");
            this.Engagement.AddItem("Лайков на пост", this.metrics.AverageLikesPerPost, ValueType.Neutral, "tooltip lp-tooltip");
            this.Engagement.AddItem("Постов на пользователей", this.metrics.InvolmentRate, ValueType.Neutral, "tooltip up-tooltip");
            this.Engagement.AddItem("Рассказать друзьям на пост", this.metrics.SharePerPost, ValueType.Neutral, "tooltip sp-tooltip");
            this.Engagement.AddItem("Наибольшая активность", mostCrowdedTime, ValueType.Default, "tooltip ma-tooltip", false);
        }
    }
}