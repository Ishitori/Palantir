namespace Ix.Palantir.Security
{
    using System;
    using System.Security.Principal;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Security.API;

    public static class PrincipalExtension
    {
         public static int GetId(this IPrincipal principal)
         {
             if (principal == null)
             {
                 return 0;
             }

             var identity = principal.Identity as CustomIdentity;

             if (identity == null)
             {
                 throw new PalantirException("Invalid security identity is provided");
             }

             return identity.Id;
         }
         public static IAccount GetAccount(this IPrincipal principal)
         {
             if (principal == null)
             {
                 return null;
             }

             var identity = principal.Identity as CustomIdentity;

             if (identity == null)
             {
                 throw new PalantirException("Invalid security identity is provided");
             }

             return identity.User.Account;
         }
        public static TimeZoneInfo GetTimeZone(this IPrincipal principal)
        {
            if (principal == null)
            {
                return TimeZoneInfo.Utc;
            }

            var identity = principal.Identity as CustomIdentity;

            if (identity == null)
            {
                throw new PalantirException("Invalid security identity is provided");
            }

            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(identity.TimeZoneName);
            return userTimeZone;
        }
    }
}