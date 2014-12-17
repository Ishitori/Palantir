namespace Ix.Palantir.DataAccess.StatisticsProviders.Helpers
{
    using System;
    using System.Linq;
    using Ix.Palantir.DomainModel;

    public static class MemberQueryHelper
    {
        public static IQueryable<Member> WithCityCriteria(this IQueryable<Member> members, string cities)
        {
            if (cities != null)
            {
                var citiesList = cities.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                members = members.Where(x => citiesList.Contains(x.CityId));
            }

            return members;
        }

        public static IQueryable<Member> WithGenderCriteria(this IQueryable<Member> members, bool male, bool female)
        {
            if (male && female)
            {
                members = members.Where(x => x.Gender != Gender.Unknown);
            }
            else if (female)
            {
                members = members.Where(x => x.Gender == Gender.Female);
            }
            else if (male)
            {
                members = members.Where(x => x.Gender == Gender.Male);
            }

            return members;
        }

        public static IQueryable<Member> WithEducationCriteria(this IQueryable<Member> members, int minEducation, int maxEducation)
        {
            var minEducationEnum = (EducationLevel)minEducation;
            var maxEducationEnum = (EducationLevel)maxEducation;
            members = members.Where(x => x.Education >= minEducationEnum && x.Education <= maxEducationEnum);
            return members;
        }

        public static IQueryable<Member> WithAgeCriteria(this IQueryable<Member> members, int minAge, int maxAge)
        {
            var currentMinYear = minAge != 0 ? DateTime.Now.Year - minAge : 0;
            var currentMaxYear = maxAge != 0 ? DateTime.Now.Year - maxAge : 0;

            if (currentMinYear != 0)
            {
                members = members.Where(x => x.BirthDate.BirthYear <= currentMinYear);
            }

            if (currentMaxYear != 0)
            {
                members = members.Where(x => x.BirthDate.BirthYear >= currentMaxYear);
            }

            return members;
        }
    }
}