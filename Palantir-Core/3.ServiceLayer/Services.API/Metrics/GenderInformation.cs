namespace Ix.Palantir.Services.API.Metrics
{
    using Ix.Palantir.Querying.Common;

    public class GenderInformation
    {
        public int Males { get; set; }
        public int Females { get; set; }
        public int Unknown { get; set; }

        public static GenderInformation Create(CategorialValue categories)
        {
            GenderInformation info = new GenderInformation();

            info.Males = categories.CategoryA;
            info.Females = categories.CategoryB;
            info.Unknown = categories.CategoryC;

            return info;
        }
    }
}