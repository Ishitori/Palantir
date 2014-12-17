namespace Ix.Palantir.UI
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.App.Bootstrapper;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.UI.Binders;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true; 
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Project.Metrics",
                "projects/{id}/{controller}/{action}",
                new { action = "Index" });

            routes.MapRoute(
                "Project.List",
                string.Empty,
                new { controller = "Home", action = "Index" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Home", action = "Index" });
        }

        protected void Application_Start()
        {
            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new ObjectFactoryControllerFactory());

            ModelBinders.Binders.Add(typeof(DataFilter), new DataFilterBinder());
            ModelBinders.Binders.Add(typeof(IList<int>), new IntListBinder());
        }
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
            {
                return;
            }

            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

            if (authTicket == null)
            {
                return;
            }

            var principal = Factory.GetInstance<IPrincipalBuilder>().CreatePrincipal(authTicket.Name);

            if (principal == null)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.RedirectToLoginPage();
                this.Response.End();
            }

            Factory.GetInstance<ICurrentUserProvider>().SetCurrentUser(principal);
        }
        protected void Application_End()
        {
            LogManager.Flush();
        }
        protected void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.GetLogger().FatalFormat("Unhandled exception: {0}", e.ExceptionObject as Exception);
            LogManager.Flush();
        }
        protected virtual void Application_Error()
        {
            Exception serverLastError = HttpContext.Current.Server.GetLastError();
            LogManager.GetLogger().FatalFormat("Unhandled exception: {0}", serverLastError);
        }
    }
}