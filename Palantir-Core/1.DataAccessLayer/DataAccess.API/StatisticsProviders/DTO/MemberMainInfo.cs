namespace Ix.Palantir.DataAccess.API.StatisticsProviders.DTO
{
    using System;
    using Ix.Palantir.DomainModel;

    [Serializable]
    public class MemberMainInfo
    {
        public virtual long VkMemberId { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual BirthDate BirthDate { get; set; }
        public virtual EducationLevel Education { get; set; }
        public virtual int CityId { get; set; }
        public virtual int CountryId { get; set; }
    }
}