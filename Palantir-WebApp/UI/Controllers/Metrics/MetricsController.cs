namespace Ix.Palantir.UI.Controllers.Metrics
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web.Mvc;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.UI.Models;

    public abstract class MetricsController : Controller
    {
        public abstract ActionResult Index(int id);

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MetricsView(string name, object model)
        {
            string viewPath = string.Format("~/Views/Metrics/{0}.cshtml", name);
            return this.View(viewPath, model);
        }

        protected ActionResult Chart(object chartData)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(chartData.GetType());
            string json;

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, chartData);
                json = Encoding.UTF8.GetString(ms.ToArray());
            }

            ContentResult result = new ContentResult()
                                       {
                                           Content = json,
                                           ContentEncoding = Encoding.UTF8,
                                           ContentType = "application/json"
                                       };
            return result;
        }
    }
}