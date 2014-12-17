namespace Ix.Palantir.DomainModel
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Member : IVkEntity, IEntity
    {
        public Member()
        {
            this.BirthDate = new BirthDate();
            this.Interests = new List<MemberInterest>();
        }

        public virtual long VkMemberId { get; set; }
        public virtual string Name { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual MemberMaritalStatus MaritalStatus { get; set; }
        public virtual BirthDate BirthDate { get; set; }
        public virtual int CityId { get; set; }
        public virtual int CountryId { get; set; }
        public virtual MemberStatus Status { get; set; }
        public virtual EducationLevel Education { get; set; }
        public virtual IList<MemberInterest> Interests { get; set; }
        public virtual int Id { get; set; }
        public virtual int VkGroupId { get; set; }
        public virtual bool IsDeleted { get; set; }

        public virtual string VkId
        {
            get { return this.VkMemberId.ToString(); }
        }

        public static bool operator ==(Member left, Member right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(Member left, Member right)
        {
            return !object.Equals(left, right);
        }

        public virtual bool Equals(Member other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.VkGroupId == this.VkGroupId && object.Equals(other.VkMemberId, this.VkMemberId);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(Member))
            {
                return false;
            }

            return this.Equals((Member)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.VkGroupId * 397) ^ this.VkMemberId.GetHashCode();
            }
        }
    }
}