using System;

namespace ApiRequest.Core.DTOs
{
    public class ApproveUserDto
    {
        public int UserId { get; set; }
        public string Status { get; set; } // "Approved" or "Rejected"
        public string? Notes { get; set; }
    }
}
