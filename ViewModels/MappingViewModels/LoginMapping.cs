using System;

namespace CreditVillageBackend
{
    public class LoginMapping : ResponseMapping<LoginConfirm>
    {

    }

    public class LoginConfirm
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}