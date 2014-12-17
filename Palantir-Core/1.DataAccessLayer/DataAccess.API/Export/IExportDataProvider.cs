namespace Ix.Palantir.DataAccess.API.Export
{
    using Ix.Palantir.Querying.Common;

    public interface IExportDataProvider
    {
        byte[] ExportToXlsx(int vkGroupId, DateRange dateRange);
    }
}