using System.ComponentModel.DataAnnotations;

namespace CreditVillageBackend
{
    public class UpdateRequest
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "Not a valid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}