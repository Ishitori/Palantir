namespace Ix.Palantir.Services.API.Metrics
{
    using Ix.Palantir.Querying.Common;

    public class EducationLevelInformation
    {
        public int Unknown { get; set; }
        public int Middle { get; set; }
        public int UncompletedHigher { get; set; }
        public int Higher { get; set; }
        public int PhD { get; set; }

        public static EducationLevelInformation Create(CategorialValue categories)
        {
            var info = new EducationLevelInformation
                {
                    Unknown = categories.CategoryA,
                    Middle = categories.CategoryB,
                    UncompletedHigher = categories.CategoryC,
                    Higher = categories.CategoryD,
                    PhD = categories.CategoryE
                };

            return info;
        }
    }
}
