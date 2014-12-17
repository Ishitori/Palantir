namespace Ix.Palantir.Querying.Common.DataFilters
{
    public class AudienceFilter
    {
        public long Code { get; set; }

        // gender
        public bool Male { get; set; }

        public bool Female { get; set; }

        // education
        public int MinEducation { get; set; }

        public int MaxEducation { get; set; }

        // age
        public int MinAge { get; set; }

        public int MaxAge { get; set; }

        // cities
        public string Cities { get; set; }
        
        public bool IsEmpty
        {
            get
            {
                return
                    !this.Male &&
                    !this.Female &&
                    this.MinEducation == 0 &&
                    this.MaxEducation == 4 &&
                    this.MinAge == 0 &&
                    this.MaxAge == 100 &&
                    string.IsNullOrWhiteSpace(this.Cities);
            }
        }
    }
}