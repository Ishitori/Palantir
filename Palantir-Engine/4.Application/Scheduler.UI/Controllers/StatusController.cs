namespace Ix.Palantir.Scheduler.UI.Controllers
{
    using System.Web.Mvc;
    using Models;

    public class StatusController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return this.View(new StatusModel());
        }

        [HttpPost]
        public ActionResult StartProcessing(StatusModel model)
        {
            if (!model.IsSchedulerRunning)
            {
                model.StartScheduler();
            }

            return this.RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult StopProcessing(StatusModel model)
        {
            if (model.IsSchedulerRunning)
            {
                model.StopScheduler();
            }

            return this.RedirectToAction("Index");
        }
    }
}