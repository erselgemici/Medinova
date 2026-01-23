using System.ComponentModel.DataAnnotations;

namespace Medinova.DTOs.AccountDtos
{
    public class LoginDto
    {       
            [Required(ErrorMessage = "E-posta adresi zorunludur.")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta formatı giriniz.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre zorunludur.")]
            public string Password { get; set; }        
    }
}
