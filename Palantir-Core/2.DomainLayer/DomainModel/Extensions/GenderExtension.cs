namespace Ix.Palantir.DomainModel.Extensions
{
    using System;

    public static class GenderExtension
    {
         public static string GetLabel(this Gender gender)
         {
             switch (gender)
             {
                 case Gender.Unknown:
                     return "Неизвестно";

                 case Gender.Male:
                     return "Мужчина";

                 case Gender.Female:
                     return "Женщина";

                 default:
                     throw new ArgumentOutOfRangeException("gender");
             }
         }
    }
}