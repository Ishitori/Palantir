namespace Ix.Palantir.Admin.UI.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using Models;

    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult DoLogin(LoginViewModel loginModel)
        {
            if (FormsAuthentication.Authenticate(loginModel.Login, loginModel.Password))
            {
                FormsAuthentication.RedirectFromLoginPage(loginModel.Login, false);
                return this.RedirectToAction("List", "Accounts");
            }

            return this.RedirectToAction("Index", "Login");
        }
    }
}