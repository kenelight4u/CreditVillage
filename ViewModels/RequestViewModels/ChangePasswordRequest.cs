using System.ComponentModel.DataAnnotations;

namespace CreditVillageBackend
{
    public class ChangePasswordRequest
    {
        [Required]
        [Display(Name = "Old Password")]
        public string Old_Password { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}