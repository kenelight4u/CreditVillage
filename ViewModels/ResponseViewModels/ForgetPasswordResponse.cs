namespace CreditVillageBackend
{  
    public class ForgetPasswordResponse
    {
        public bool Check { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }
    }
}