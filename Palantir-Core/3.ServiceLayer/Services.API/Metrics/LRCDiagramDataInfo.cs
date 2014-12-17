namespace Ix.Palantir.Services.API.Metrics
{
    using Ix.Palantir.Querying.Common;

    public class LRCDiagramDataInfo
    {
        public int Likes { get; set; }
        public int Comments { get; set; }
        public int Reposts { get; set; }
        public int Posts { get; set; }
        /*public int Photos { get; set; }
        public int Videos { get; set; }*/

         public static LRCDiagramDataInfo Create(CategorialValue categories)
         {
             return new LRCDiagramDataInfo()
                 {
                     Comments = categories.CategoryA,
                     Reposts = categories.CategoryB,
                     Likes = categories.CategoryC, 
                     Posts = categories.CategoryD,
                     /*Photos = categories.CategoryE,
                     Videos = categories.CategoryF*/
                 };
         }
    }
}
