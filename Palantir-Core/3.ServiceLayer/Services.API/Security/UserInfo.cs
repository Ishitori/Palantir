namespace Ix.Palantir.Services.API.Security
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Ix.Palantir.Security.API;

    public class UserInfo
    {
        public UserInfo()
        {
        }
        public UserInfo(User user)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "* Поле обязательно к заполнению")]
        [DisplayName("Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "* Поле обязательно к заполнению")]
        [DisplayName("Фамилия")]
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        [Required(ErrorMessage = "* Поле обязательно к заполнению")]
        [DisplayName("Емейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* Поле обязательно к заполнению")]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}