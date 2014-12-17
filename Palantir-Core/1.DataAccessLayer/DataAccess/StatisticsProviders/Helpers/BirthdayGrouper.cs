namespace Ix.Palantir.DataAccess.StatisticsProviders.Helpers
{
    using System.Collections.Generic;

    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Querying.Common;

    public class BirthdayGrouper
    {
        public CategorialValue GroupByBirthday(IEnumerable<BirthDate> birthdays, bool withoutMonthAndDay = false)
        {
            CategorialValue value = new CategorialValue();

            foreach (var member in birthdays)
            {
                int age = !withoutMonthAndDay ? member.GetAge() : member.GetAgeWithoutMonthAndDay();

                if (age >= 1 && age < 14)
                {
                    value.CategoryA++;
                }
                else if (age >= 15 && age <= 24)
                {
                    value.CategoryB++;
                }
                else if (age >= 25 && age <= 34)
                {
                    value.CategoryC++;
                }
                else if (age >= 35 && age <= 44)
                {
                    value.CategoryD++;
                }
                else if (age >= 45 && age <= 54)
                {
                    value.CategoryE++;
                }
                else if (age >= 55)
                {
                    value.CategoryF++;
                }
                else
                {
                    value.CategoryG++;
                }
            }

            return value;
        }
    }
}