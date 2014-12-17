namespace Ix.Palantir.Services.API.Metrics
{
    using Ix.Palantir.Querying.Common;

    public class AgeInformation
    {
        public int Below14 { get; set; }
        public int Upper15Below24 { get; set; }
        public int Upper25Below34 { get; set; }
        public int Upper35Below44 { get; set; }
        public int Upper45Below54 { get; set; }
        public int Upper55 { get; set; }
        public int Unknown { get; set; }

        public static AgeInformation Create(CategorialValue categories)
        {
            AgeInformation ageInfo = new AgeInformation();

            ageInfo.Below14 = categories.CategoryA;
            ageInfo.Upper15Below24 = categories.CategoryB;
            ageInfo.Upper25Below34 = categories.CategoryC;
            ageInfo.Upper35Below44 = categories.CategoryD;
            ageInfo.Upper45Below54 = categories.CategoryE;
            ageInfo.Upper55 = categories.CategoryF;
            ageInfo.Unknown = categories.CategoryG;

            return ageInfo;
        }
    }
}