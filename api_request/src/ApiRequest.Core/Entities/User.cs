namespace ApiRequest.Core.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string AccountType { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public string IdImage { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string TradeLicenseImage { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime OtpGeneratedAt { get; set; }
        public int Otp { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected



        // Navigation properties
        public ICollection<Request> Requests { get; set; }
        public ICollection<UserToken> UserTokens { get; set; }
    }
}
