namespace Ix.Palantir.Services.API.Metrics
{
    using Ix.Palantir.Querying.Common;

    public class TypeOfContentDataInfo
    {
        public int Text { get; set; }
        public int Video { get; set; }
        public int Photo { get; set; }
        public int Link { get; set; }

        public static TypeOfContentDataInfo Create(CategorialValue categories)
        {
            return new TypeOfContentDataInfo()
            {
                Text = categories.CategoryA,
                Video = categories.CategoryB,
                Photo = categories.CategoryC,
                Link = categories.CategoryD
            };
        }
    }
}
