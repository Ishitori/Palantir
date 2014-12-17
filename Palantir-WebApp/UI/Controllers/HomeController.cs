namespace Ix.Palantir.UI.Controllers
{
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (this.Request.IsAuthenticated)
            {
                return this.RedirectToAction("List", "Projects");
            }
            return this.View();
        }
    }
}
