namespace Ix.Palantir.DomainModel
{
    using System;

    [Serializable]
    public class MemberInterest : IEntity
    {
        public virtual int Id { get; set; }
        public virtual int VkGroupId { get; set; }
        public virtual long VkMemberId { get; set; }
        public virtual string Title { get; set; }
        public virtual MemberInterestType Type { get; set; }

        public static bool operator ==(MemberInterest left, MemberInterest right)
        {
            return object.Equals(left, right);
        }
        public static bool operator !=(MemberInterest left, MemberInterest right)
        {
            return !object.Equals(left, right);
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((MemberInterest)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.VkGroupId;
                hashCode = (hashCode * 397) ^ this.VkMemberId.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.Title != null ? this.Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Type.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(MemberInterest other)
        {
            return this.VkGroupId == other.VkGroupId && string.Equals(this.VkMemberId, other.VkMemberId) && string.Equals(this.Title, other.Title) && this.Type.Equals(other.Type);
        }
    }
}