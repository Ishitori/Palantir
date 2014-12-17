namespace Ix.Palantir.DomainModel.Extensions
{
    using System;

    public static class MemberStatusExtension
    {
         public static string GetLabel(this MemberStatus status)
         {
             switch (status)
             {
                 case MemberStatus.Active:
                     return "Активный";

                 case MemberStatus.Blocked:
                     return "Заблокированный";

                 case MemberStatus.Deleted:
                     return "Удаленные";

                 case MemberStatus.Bot:
                     return "Бот";

                 default:
                     throw new ArgumentOutOfRangeException("status");
             }
         }
    }
}