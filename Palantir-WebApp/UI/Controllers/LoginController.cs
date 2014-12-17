namespace Ix.Palantir.UI.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Ix.Palantir.Localization.API;
    using Models.Login;
    using Services.API;

    public class LoginController : Controller
    {
        private readonly IUserService userService;
        private readonly IDateTimeHelper dateTimeHelper;

        public LoginController(IUserService userService, IDateTimeHelper dateTimeHelper)
        {
            this.userService = userService;
            this.dateTimeHelper = dateTimeHelper;
        }

        public ActionResult Index()
        {
            if (this.Request.IsAuthenticated)
            {
                return new RedirectResult("/");
            }

            return this.View();
        }
        [HttpPost]
        public ActionResult DoLogin(LoginViewModel model)
        {
            bool isUserValid = this.userService.IsUserValid(model.Login, model.Password);

            if (!isUserValid)
            {
                return new RedirectResult("/Login/");
            }

            var authTicket = new FormsAuthenticationTicket(1, model.Login, this.dateTimeHelper.GetDateTimeNow(), this.dateTimeHelper.GetDateTimeNow().AddDays(7), true, string.Empty);
            string cookieContents = FormsAuthentication.Encrypt(authTicket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieContents)
            {
                Expires = authTicket.Expiration,
                Path = FormsAuthentication.FormsCookiePath
            };
            
            this.HttpContext.Response.Cookies.Add(cookie);
            return new RedirectResult("/");
        }
        [HttpGet]
        public ActionResult DoLogout()
        {
            FormsAuthentication.SignOut();
            return new RedirectResult("/");
        }
    }
}