namespace Ix.Palantir.DataAccess.API.StatisticsProviders
{
    using Ix.Palantir.DataAccess.API.StatisticsProviders.DTO;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Querying.Common.DataFilters;

    public interface IKpiProvider
    {
        double GetAverageLikesPerPost(int projectId, DateRange dateRange);
        double GetInteractionRate(int projectId, DateRange dateRange);
        double GetResponseRate(int projectId, DateRange dateRange);
        double GetInvolmentRate(int projectId, DateRange dateRange);
        double GetUgcRate(int projectId, DateRange dateRange);

        Kpi GetKpis(int projectId, DateRange dateRange);
    }
}