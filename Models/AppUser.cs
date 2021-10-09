using System;
using System.ComponentModel.DataAnnotations.Schema;
using CreditVillageBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace CreditVillageBackend
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        //public string Full_Name { get; set; }

        //public string Address { get; set; }

        //public string Bio { get; set; }

        // public string Image_Url { get; set; }

        public Guid LogoFileId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        //public int NationalityId { get; set; }

        //public int StateId { get; set; }

        //public Nationality Nationality { get; set; }

        //public State State { get; set; }
    }
}