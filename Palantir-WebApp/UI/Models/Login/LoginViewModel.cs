namespace Ix.Palantir.UI.Models.Login
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [DisplayName("Емейл:")]
        public string Login { get; set; }
        [DisplayName("Пароль:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}