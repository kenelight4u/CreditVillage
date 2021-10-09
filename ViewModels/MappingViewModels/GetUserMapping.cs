using System;

namespace CreditVillageBackend
{
   
    public class GetUserMapping 
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string FileFullName { get; set; }

        public string FileBase64 { get; set; }   
    }

    public class DetailsConfirm
    {
        public string FullName { get; set; }

        public string File { get; set; }
    }
}