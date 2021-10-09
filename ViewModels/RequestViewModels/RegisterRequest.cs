using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CreditVillageBackend
{
    public class RegisterRequest
    {
        //[Required]
        //[Display(Name = "Full Name")]
        //public string Full_Name { get; set; }


        //[Required]
        //[Display(Name = "Nationality")]
        //public int Nationality { get; set; }


        //[Required]
        //[Display(Name = "State")]
        //public int State { get; set; }


        [Required]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        //[Required]
        //[Display(Name = "Email Confirm Address")]
        //[DataType(DataType.EmailAddress)]
        //[Compare("Email", ErrorMessage = "The Email do not match.")]
        //public string Confirm_Email { get; set; }


        //[Required]
        //[Display(Name = "Phone Number")]
        //public string PhoneNumber { get; set; }


        //[Required]
        //[Compare("Phone_Number", ErrorMessage = "The phone number do not match.")]
        //[Display(Name = "Confirm Phone Number")]
        //public string Confirm_Phone_Number { get; set; }


        [Required]
        [Display(Name = "Password")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password do not match.")]
        [Display(Name = "Confirm Password")]
        public string Confirm_Password { get; set; }

    }
}