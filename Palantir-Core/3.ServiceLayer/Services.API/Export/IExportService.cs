namespace Ix.Palantir.Services.API.Export
{
    using Ix.Palantir.Querying.Common;

    public interface IExportService
    {
        ExportSchedulingResult ScheduleExport(int projectId, DateRange dateRange);
        ExportExecutionResult GetExportStatus(int projectId, string ticketId);
        byte[] LoadExportFile(int projectId, string virtualFilePath);
    }
}