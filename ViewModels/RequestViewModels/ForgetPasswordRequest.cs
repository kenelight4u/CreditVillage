using System.ComponentModel.DataAnnotations;

namespace CreditVillageBackend
{
    public class ForgetPasswordRequest
    {
        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}