using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } // 👇 Yeni eklendi (Veritabanı istiyor)

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } // 👇 Yeni eklendi

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz.")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } // 👇 Yeni eklendi

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor!")]
        public string ConfirmPassword { get; set; }
    }
}