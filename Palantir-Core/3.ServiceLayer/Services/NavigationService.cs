namespace Ix.Palantir.Services
{
    using System.Collections.Generic;
    using System.Web.Routing;
    using Ix.Palantir.Services.API;

    public class NavigationService : INavigationService
    {
        public IEnumerable<MenuItemLink> GetMenuItems()
        {
            IList<MenuItemLink> menu = new List<MenuItemLink>
            {
                new MenuItemLink
                {
                    Title = "Главная страница",
                    RouteValues = new RouteValueDictionary(new { controller = "projects", action = "list" }),
                }
            };

            return menu;
        }

        public IEnumerable<MenuItemLink> GetMetricsMenuItems()
        {
            return new List<MenuItemLink>
            {
                new MenuItemLink
                {
                    Title = "Метрики",
                    RouteValues = new RouteValueDictionary(new { controller = "dashboard" }),
                    ImgSrc = "/Content/images/dashboard.png"
                },
                new MenuItemLink
                {
                    Title = "Популярные посты",
                    RouteValues = new RouteValueDictionary(new { controller = "posts" }),
                    ImgSrc = "/Content/images/posts.png"
                },
                new MenuItemLink
                {
                    Title = "Анализ медиа",
                    RouteValues = new RouteValueDictionary(new { controller = "content" }),
                    ImgSrc = "/Content/images/bar-chart.png"
                },
                new MenuItemLink
                {
                    Title = "Анализ аудитории",
                    RouteValues = new RouteValueDictionary(new { controller = "social" }),
                    ImgSrc = "/Content/images/pie-chart.png"
                },
                new MenuItemLink
                {
                    Title = "Интересы аудитории",
                    RouteValues = new RouteValueDictionary(new { controller = "interests" }),
                    ImgSrc = "/Content/images/heart.png"
                },
                new MenuItemLink
                {
                    Title = "Анализ целевой аудитории",
                    RouteValues = new RouteValueDictionary(new { controller = "audience" }),
                    ImgSrc = "/Content/images/target_users.png"
                },
                new MenuItemLink
                {
                    Title = "Анализ вовлеченности",
                    RouteValues = new RouteValueDictionary(new { controller = "interactionrate" }),
                    ImgSrc = "/Content/images/interaction-rate.png"
                },
                new MenuItemLink
                {
                    Title = "Наиболее активные пользователи",
                    RouteValues = new RouteValueDictionary(new { controller = "activeusers" }),
                    ImgSrc = "/Content/images/most-active-users.png"
                },
                new MenuItemLink
                {
                    Title = "Анализ конкурентов",
                    RouteValues = new RouteValueDictionary(new { controller = "comparegroups" }),
                    ImgSrc = "/Content/images/compare-groups.png"
                },
                new MenuItemLink
                {
                    Title = "Экспорт данных",
                    RouteValues = new RouteValueDictionary(new { controller = "export" }),
                    ImgSrc = "/Content/images/export.png"
                },
                new MenuItemLink
                {
                    Title = "Управление группой",
                    RouteValues = new RouteValueDictionary(new { controller = "settings" }),
                    ImgSrc = "/Content/images/settings.png"
                }
            };
        }
    }
}