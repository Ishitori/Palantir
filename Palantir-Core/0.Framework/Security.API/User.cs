namespace Ix.Palantir.Security.API
{
    using System.ComponentModel;

    public class User
    {
        public virtual int Id { get; set; }
        [DisplayName("Имя")]
        public virtual string FirstName { get; set; }
        [DisplayName("Фамилия")]
        public virtual string LastName { get; set; }
        public virtual string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }
        [DisplayName("E-mail")]
        public virtual string Email { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual int Salt { get; set; }
        public virtual string TimeZoneName
        {
            get
            {
                return "Russian Standard Time";
            }
        }

        public virtual IAccount Account { get; set; }
    }
}