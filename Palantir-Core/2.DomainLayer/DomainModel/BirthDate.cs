namespace Ix.Palantir.DomainModel
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;

    [Serializable]
    public class BirthDate
    {
        public BirthDate()
        {
        }
        public BirthDate(int birthYear, int birthMonth, int birthDay)
        {
            this.BirthYear = birthYear;
            this.BirthMonth = birthMonth;
            this.BirthDay = birthDay;
        }

        public virtual int BirthDay { get; set; }
        public virtual int BirthMonth { get; set; }
        public virtual int BirthYear { get; set; }

        public virtual int GetAge()
        {
            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();

            if (this.BirthYear <= 1900 || this.BirthYear > dateTimeHelper.GetDateTimeNow().Year)
            {
                return -1;
            }

            int age = dateTimeHelper.GetDateTimeNow().Year - this.BirthYear;

            if (this.BirthMonth > 0 && dateTimeHelper.GetDateTimeNow().Month < this.BirthMonth)
            {
                age--;
            }

            return age;
        }

        public virtual int GetAgeWithoutMonthAndDay()
        {
            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();

            if (this.BirthYear <= 1900 || this.BirthYear > dateTimeHelper.GetDateTimeNow().Year)
            {
                return -1;
            }
            int age = dateTimeHelper.GetDateTimeNow().Year - this.BirthYear;

            return age;
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((BirthDate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.BirthDay;
                hashCode = (hashCode * 397) ^ this.BirthMonth;
                hashCode = (hashCode * 397) ^ this.BirthYear;
                return hashCode;
            }
        }

        protected bool Equals(BirthDate other)
        {
            return this.BirthDay == other.BirthDay && this.BirthMonth == other.BirthMonth && this.BirthYear == other.BirthYear;
        }
    }
}