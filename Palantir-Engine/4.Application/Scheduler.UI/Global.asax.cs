namespace Ix.Palantir.Scheduler.UI
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Ix.Palantir.Engine.Bootstrapper;
    using Logging;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Status", action = "Index", id = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new ObjectFactoryControllerFactory());
            ScheduleRunner.ConfigurationFilePath = this.Server.MapPath(ConfigurationManager.AppSettings["SchedulerSettingsFilePath"]);

            ScheduleRunner.Instance.Start();
        }
        protected void Application_End()
        {
            if (ScheduleRunner.Instance.IsSchedulerRunning)
            {
                ScheduleRunner.Instance.Stop();
            }

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