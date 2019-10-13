using System.ComponentModel.DataAnnotations;

namespace bookstore.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength:100, MinimumLength = 2)]
        [Display(Name ="First name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string  PasswordConfirm { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Compare("Email")]
        public string EmailConfirm { get; set; }
        [Phone]
        public string Phonenumber { get; set; }
    }
}
