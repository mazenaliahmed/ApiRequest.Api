namespace ApiRequest.Core.Entities
{
    public class Request
    {
        public long Id { get; set; }
        public string RequestNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime EntryTime { get; set; }
        public string Notes { get; set; }
        public long UserId { get; set; }
        public string RequestStatus { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<RequestAssignment> RequestAssignments { get; set; }
        public ICollection<RequestDocument> RequestDocuments { get; set; }

    }
}
