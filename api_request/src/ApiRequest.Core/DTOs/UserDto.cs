using Microsoft.AspNetCore.Http;

namespace ApiRequest.Core.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string IdImage { get; set; }
        public string TradeLicenseImage { get; set; }
        public byte[] PasswordSalt { get; set; }


    }

    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string AccountType { get; set; }
        public string PasswordSalt { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IFormFile IdImage { get; set; }
        public IFormFile TradeLicenseImage { get; set; }
    }

    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
