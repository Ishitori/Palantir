namespace Ix.Palantir.UI.Controllers
{
    using System.Web.Mvc;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Export;
    using Ix.Palantir.Services.API.Metrics;
    using Ix.Palantir.UI.Models.Converters;
    using Ix.Palantir.UI.Models.Metrics;

    public class ExportController : Controller
    {
        private readonly IMetricsService metricsService;
        private readonly IExportService exportService;

        public ExportController(IMetricsService metricsService, IExportService exportService)
        {
            this.metricsService = metricsService;
            this.exportService = exportService;
        }

        public ActionResult Index(int id)
        {
            MetricsBase metrics = this.metricsService.GetBaseMetrics(id);
            MetricsViewModel viewModel = new MetricsViewModel(metrics);

            return this.View(viewModel);
        }

        public ActionResult ExportData(int id, DataFilter filter)
        {
            DateRange dateRange = DateRangeConverter.GetDateRange(filter);
            ExportSchedulingResult result = this.exportService.ScheduleExport(id, dateRange);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IsReady(int id, string ticketId)
        {
            ExportExecutionResult result = this.exportService.GetExportStatus(id, ticketId);

            if (result.IsSuccess)
            {
                result.FileUrl = Url.Action("DownloadFile", "Export", new { fileName = result.FileUrl });
            }

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(int id, string fileName)
        {
            byte[] bytes = this.exportService.LoadExportFile(id, fileName);
            int slashIndex = fileName.LastIndexOf("\\", System.StringComparison.Ordinal);
            string downloadName = slashIndex != -1
                                      ? fileName.Substring(slashIndex + 1, fileName.Length - slashIndex - 1)
                                      : "export_result.xlsx";
            return this.File(bytes, "application/vnd.ms-excel", downloadName);
        }
    }
}
