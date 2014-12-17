namespace Ix.Palantir.DomainModel.Extensions
{
    using System;

    public static class EducationExtension
    {
         public static string GetLabel(this EducationLevel education)
         {
             switch (education)
             {
                 case EducationLevel.Unknown:
                     return "Неизвестно";

                 case EducationLevel.Middle:
                     return "Среднее";

                 case EducationLevel.UncompletedHigher:
                     return "Неоконченное высшее";

                 case EducationLevel.Higher:
                     return "Высшее";

                 case EducationLevel.PhD:
                     return "Кандидат или доктор наук";

                 default:
                     throw new ArgumentOutOfRangeException("education");
             }
         }
    }
}