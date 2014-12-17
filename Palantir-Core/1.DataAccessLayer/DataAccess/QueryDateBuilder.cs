namespace Ix.Palantir.DataAccess
{
    using System;

    public static class QueryDateBuilder
    {
        public static string GetDateString(DateTime time)
        {
            return string.Format("{0}-{1}-{2}", time.Year, time.Month, time.Day);
        }
    }
}
