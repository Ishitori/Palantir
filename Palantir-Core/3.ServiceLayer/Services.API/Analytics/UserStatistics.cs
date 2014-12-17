namespace Ix.Palantir.Services.API.Analytics
{
    using System.Collections.Generic;

    public class UserStatistics
    {
        public IList<long> AllUsers { get; set; }
        public IList<long> ActiveUsers { get; set; }
        public IList<long> InactiveUsers { get; set; }
        public IList<long> Bots { get; set; }
        public IList<long> DeletedUsers { get; set; }
        public IList<long> BlockedUsers { get; set; }
    }
}