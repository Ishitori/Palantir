namespace Ix.Palantir.DomainModel
{
    public class ActiveUser
    {
        public virtual long Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual int CommentCount { get; set; }
        public virtual int PostCount { get; set; }
        public virtual int LikeCount { get; set; }
        public virtual int ShareCount { get; set; }
        
        public virtual Gender Gender { get; set; }
        public virtual BirthDate BirthDate { get; set; }
        public virtual EducationLevel Education { get; set; }
        public virtual int CityId { get; set; }
        public virtual int CountryId { get; set; }
    }
}
