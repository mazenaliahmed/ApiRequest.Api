using Microsoft.AspNetCore.Http;

namespace ApiRequest.Core.DTOs
{
    public class RequestDto
    {
        public long Id { get; set; }
        public string RequestNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public string Notes { get; set; }
        public string RequestStatus { get; set; }
        public long UserId { get; set; }
    }

    public class CreateRequestDto
    {

        public string RequestNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public string Notes { get; set; }
        public string DocumentName { get; set; }

        public List<IFormFile> File { get; set; }
        //public List<RequestDocumentDto> Documents { get; set; }
    }

    public class CreateDocumentDto
    {
        public string DocumentName { get; set; }
        public IFormFile File { get; set; }
        // يمكنك إضافة حقول أخرى إذا لزم الأمر
    }


    public class RequestDocumentDto
    {
        public string DocumentName { get; set; }
        public string AccountType { get; set; }
        public byte[]? DocumentContent { get; set; }



        public IFormFile File { get; set; }
        // يمكنك إضافة حقول أخرى إذا لزم الأمر


    }

    public class AssignRequestDto
    {
        public long RequestId { get; set; }
        public long ServiceProviderId { get; set; }
    }

    public class UpdateRequestStatusDto
    {
        public string ExecutionStatus { get; set; }
    }
}
