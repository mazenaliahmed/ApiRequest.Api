namespace ApiRequest.Core.DTOs
{
    public class MediatorRequestDto
    {
        public long RequestId { get; set; }
        public string RequestNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public string Notes { get; set; }
        public string RequestStatus { get; set; }
        public long UserId { get; set; }
        public long? ServiceProviderId { get; set; }
    }

    public class ServiceProviderAssignmentDto
    {
        public long RequestId { get; set; }
        public long ServiceProviderId { get; set; }
        public string ExecutionStatus { get; set; }
    }

    public class ServiceProviderListItemDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
