namespace ApiRequest.Core.Entities
{
    public class RequestAssignment
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public long ServiceProviderId { get; set; }
        public string ExecutionStatus { get; set; }

        // Navigation properties
        public Request Request { get; set; }
    }
}
