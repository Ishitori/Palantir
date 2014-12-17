namespace Ix.Palantir.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Services.API;

    public class NavigationController : Controller
    {
        private const string CONST_ControllerKey = "controller";
        private readonly INavigationService navigationService;

        public NavigationController(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        [ChildActionOnly]
        public ActionResult MetricsMenu()
        {            
            IEnumerable<MenuItemLink> menuItems = this.navigationService.GetMetricsMenuItems();
            this.MarkSelectedItem(menuItems);
            return this.View("Menu", menuItems);
        }

        private void MarkSelectedItem(IEnumerable<MenuItemLink> menuItems)
        {
            var currentController = this.Request.RequestContext.RouteData.Values[CONST_ControllerKey] as string;

            foreach (var menuItem in menuItems)
            {
                var routeController = menuItem.RouteValues[CONST_ControllerKey] as string;

                if (string.Compare(routeController, currentController, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    menuItem.IsSelected = true;
                    return;
                }
            }
        }
    }
}
