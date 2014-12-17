namespace Ix.Palantir.Utilities
{
    using System;

    public static class UnixEpoh
    {
        public static DateTime Create()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}