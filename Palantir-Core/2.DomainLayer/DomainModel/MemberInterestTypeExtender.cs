namespace Ix.Palantir.DomainModel
{
    public static class MemberInterestTypeExtender
    {
         public static string GetTile(this MemberInterestType interestType)
         {
             switch (interestType)
             {
                 case MemberInterestType.General:
                     return "Общее";

                 case MemberInterestType.Movie:
                     return "Видео";

                 case MemberInterestType.TVShow:
                     return "ТВШоу";

                 case MemberInterestType.Book:
                     return "Книги";

                 case MemberInterestType.VideoGame:
                     return "Игры";

                 default:
                     return string.Empty;
             }
         }
    }
}