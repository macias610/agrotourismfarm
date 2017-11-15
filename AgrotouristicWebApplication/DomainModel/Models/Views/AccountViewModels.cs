using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repository.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage ="Pole email jest wymagane")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="Niepoprawny format adresu e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Pole hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Zampamiętaj mnie?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Pole Email jest wymagane")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Pole Imię jest wymagane")]
        [MinLength(3, ErrorMessage = "Pole Imię musi mieć minimum 3 znaki"),MaxLength(20, ErrorMessage = "Pole Imię musi mieć max 20 znaków")]
        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole Nazwisko jest wymagane")]
        [MinLength(3,ErrorMessage ="Pole Nazwisko musi mieć minimum 3 znaki"),MaxLength(20, ErrorMessage = "Pole Nazwisko musi mieć max 20 znaków")]
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Pole Data urodzenia jest wymagane")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name="Data urodzenia")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Pole Telefon jest wymagane")]
        [StringLength(9,ErrorMessage ="Telefon musi składać się z 9 liczb")]
        [Display(Name="Telefon")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telefon musi składać tylko z liczb")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Pole Hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie nie są identyczne.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="Pole Email jest wymagane")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Pole Hasło jest wymagane")]
        [StringLength(100, ErrorMessage = "{0} musi mieć przynajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie nie są identyczne.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="Pole Email jest wymagane")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
