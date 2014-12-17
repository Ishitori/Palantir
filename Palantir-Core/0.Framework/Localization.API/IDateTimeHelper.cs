namespace Ix.Palantir.Localization.API
{
    using System;

    public interface IDateTimeHelper
    {
        DateTime GetDateTimeNow();
        DateTime GetDateNoTimeNow();
        DateTime GetEmptyDateTime();

        DateTime GetUtcDate(DateTime localDateTime);
        DateTime GetLocalUserDate(DateTime utcDateTime);
        TimeFrame GetLocalUserTimeFrame(TimeFrame utcTimeFrame);
    }
}