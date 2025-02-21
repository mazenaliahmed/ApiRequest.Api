namespace ApiRequest.Core.DTOs
{
    public class ServiceProviderTaskDto
    {
        public long AssignmentId { get; set; }
        public string RequestNumber { get; set; }
        public string ExecutionStatus { get; set; }
        public List<DocumentDto> Documents { get; set; }
    }

    public class DocumentDto
    {
        public long Id { get; set; }
        public string DocumentName { get; set; }
        public string AccountType { get; set; }
        public DateTime UploadTime { get; set; }
        public string DocumentContent { get; set; }
    }

    public class UpdateTaskStatusDto
    {
        public long AssignmentId { get; set; }
        public string ExecutionStatus { get; set; }
        public List<DocumentDto> NewDocuments { get; set; }
    }
}
