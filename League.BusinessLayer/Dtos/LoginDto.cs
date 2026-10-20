using System.ComponentModel.DataAnnotations;

namespace League.BusinessLayer.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Kullanıcı adını giriniz.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifreyi giriniz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
