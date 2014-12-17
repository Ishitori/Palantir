namespace Ix.Palantir.DomainModel.Extensions
{
    using System;

    public static class MaritalStatusExtension
    {
        public static string GetLabel(this MemberMaritalStatus maritalStatus)
        {
            switch (maritalStatus)
            {
                case MemberMaritalStatus.Unknown:
                    return "Неизвестно";

                case MemberMaritalStatus.Single:
                    return "Холост / Незамужем";

                case MemberMaritalStatus.Married:
                    return "Женат / Замужем";

                default:
                    throw new ArgumentOutOfRangeException("maritalStatus");
            }
        }
    }
}