using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CreditVillageBackend.ViewModels.RequestViewModels
{
    public class EditRequest
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [RegularExpression(@"^([0-9]{11})$", ErrorMessage = "Not a valid phone number")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public IFormFile LogoFile { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }
    }
}
