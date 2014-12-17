namespace Ix.Palantir.Vkontakte.Workflows.VkMappers
{
    using System;
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public class MemberInterestEqualityComparer : IEqualityComparer<MemberInterest>
    {
        private const string CONST_KeyTemplate = "{0}-{1}";

        public bool Equals(MemberInterest x, MemberInterest y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return string.Compare(string.Format(CONST_KeyTemplate, x.Title.ToLower(), x.Type), string.Format(CONST_KeyTemplate, y.Title.ToLower(), y.Type), StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        public int GetHashCode(MemberInterest obj)
        {
            return string.Format(CONST_KeyTemplate, obj.Title.ToLower(), obj.Type).GetHashCode();
        }
    }
}