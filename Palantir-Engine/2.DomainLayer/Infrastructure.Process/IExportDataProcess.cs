namespace Ix.Palantir.Infrastructure.Process
{
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public interface IExportDataProcess
    {
        string ScheduleExport(VkGroup vkGroup, DateRange dateRange, int initiatorUserId);
        void ProcessExportQueue();
        ExportResultCommand GetExportResult(string ticketId);
    }
}