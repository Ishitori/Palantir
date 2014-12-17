namespace Ix.Palantir.DataAccess
{
    using System;
    using System.Text;
    using Ix.Palantir.Querying.Common.DataFilters;

    public static class QueryMemberFilterBuilder
    {
        public static string GetString(int groupId, AudienceFilter filter)
        {
            StringBuilder str = new StringBuilder("SELECT vkmemberid, gender, countryid, cityid, education, birthday, birthmonth, birthyear FROM member WHERE vkgroupid = ");
            str.Append(groupId);

            if (filter.IsEmpty)
            {
                return str.ToString();
            }

            DateTime now = DateTime.Now;
            DateTime from = new DateTime(now.Year - filter.MinAge, now.Month, now.Day);
            DateTime to = new DateTime(now.Year - filter.MaxAge, now.Month, now.Day);
            if (filter.MinAge != 0 || filter.MaxAge != 100)
            {
                if (filter.MaxAge != 100 && filter.MinAge != 0)
                {
                    str.Append(" AND birthyear <= ").Append(from.Year).Append(" AND birthyear >=").Append(to.Year);
                }
                else
                {
                    if (filter.MaxAge != 100)
                    {
                        str.Append(" AND birthyear >= ").Append(to.Year);
                    }
                    else
                    {
                        str.Append(" AND birthyear <= ").Append(from.Year).Append(" AND birthyear <> 0");
                    }
                }
            }

            str.Append(" AND education >= ")
               .Append(filter.MinEducation)
               .Append(" AND education <= ")
               .Append(filter.MaxEducation);

            if (filter.Female && filter.Male)
            {
                str.Append(" AND gender <> 0");
            }
            else
            {
                if (filter.Male)
                {
                    str.Append(" AND gender = ").Append(1);
                }

                if (filter.Female)
                {
                    str.Append(" AND gender = ").Append(2);
                }
            }

            if (filter.Cities != null)
            {
                str.Append(" AND cityid = ANY(array[").Append(filter.Cities).Append("])");
            }

            return str.ToString();
        }
    }
}
