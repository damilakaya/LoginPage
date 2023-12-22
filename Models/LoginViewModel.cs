using System.ComponentModel.DataAnnotations;

namespace STB.MAP.UI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string? Password { get; set; }
    }
}
