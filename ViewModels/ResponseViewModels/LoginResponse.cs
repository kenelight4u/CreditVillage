using System;

namespace CreditVillageBackend
{
    public class LoginResponse
    {
        public bool Check { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}