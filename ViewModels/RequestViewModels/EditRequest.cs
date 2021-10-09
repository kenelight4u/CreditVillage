using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CreditVillageBackend.ViewModels.RequestViewModels
{
    public class EditRequest : UpdateRequest
    {
        
        public IFormFile LogoFile { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }
    }
}
