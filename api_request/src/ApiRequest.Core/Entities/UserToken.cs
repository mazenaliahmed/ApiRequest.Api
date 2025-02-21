using System;

namespace ApiRequest.Core.Entities
{
    public class UserToken
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Purpose { get; set; }

        // Navigation properties
        public User User { get; set; }
    }
}
