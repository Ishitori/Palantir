namespace Ix.Palantir.Services.API
{
    using Ix.Palantir.Services.API.DTO;

    public class ActiveUserInfo
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int NumberOfPosts { get; set; }
        public int NumberOfComments { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfShares { get; set; }
        public int Sum { get; set; }

        public virtual Gender Gender { get; set; }
        public virtual BirthDate BirthDate { get; set; }
        public virtual EducationLevel Education { get; set; }
        public virtual int CityId { get; set; }
        public virtual int CountryId { get; set; }
    }
}
