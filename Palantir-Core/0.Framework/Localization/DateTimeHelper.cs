namespace Ix.Palantir.Localization
{
    using System;
    using System.Security.Principal;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Security;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Utilities;

    public class DateTimeHelper : IDateTimeHelper
    {
        private const int CONST_DaysInWeek = 7;

        private readonly ICurrentUserProvider currentUserProvider;

        public DateTimeHelper(ICurrentUserProvider currentUserProvider)
        {
            this.currentUserProvider = currentUserProvider;
        }

        public DateTime GetDateTimeNow()
        {
            return DateTime.UtcNow;
        }
        public DateTime GetDateNoTimeNow()
        {
            return DateTime.UtcNow.Date;
        }
        public DateTime GetEmptyDateTime()
        {
            return new DateTime();
        }

        public DateTime GetUtcDate(DateTime localDateTime)
        {
            IPrincipal currentUser = this.currentUserProvider.GetCurrentUser();
            TimeZoneInfo timeZoneInfo = currentUser.GetTimeZone();
            return localDateTime.ToUtcDate(timeZoneInfo);
        }
        public DateTime GetLocalUserDate(DateTime utcDateTime)
        {
            IPrincipal currentUser = this.currentUserProvider.GetCurrentUser();
            TimeZoneInfo userTimeZone = currentUser.GetTimeZone();
            return utcDateTime.ToLocalUserDate(userTimeZone);
        }
        public TimeFrame GetLocalUserTimeFrame(TimeFrame utcTimeFrame)
        {
            IPrincipal currentUser = this.currentUserProvider.GetCurrentUser();
            TimeZoneInfo userTimeZone = currentUser.GetTimeZone();

            TimeSpan utcOffset = userTimeZone.BaseUtcOffset;
            TimeFrame localUserFrame = new TimeFrame
                                  {
                                      DayOfWeek = utcTimeFrame.DayOfWeek,
                                      BeginHour = utcTimeFrame.BeginHour + utcOffset.Hours,
                                      EndHour = utcTimeFrame.EndHour + utcOffset.Hours
                                  };

            if (localUserFrame.BeginHour >= 24)
            {
                localUserFrame.DayOfWeek = (DayOfWeek)(((int)localUserFrame.DayOfWeek + 1) % CONST_DaysInWeek);
                localUserFrame.BeginHour -= 24;
                localUserFrame.EndHour -= 24;
            }

            if (localUserFrame.EndHour >= 24)
            {
                localUserFrame.EndHour -= 24;
            }

            return localUserFrame;
        }
    }
}