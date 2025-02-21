public class RequestWithDocumentsDto
{
    public long Id { get; set; }
    public string RequestNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime EntryTime { get; set; }
    public string Notes { get; set; }
    public long UserId { get; set; }
    public string RequestStatus { get; set; }
    public List<RequestDocumentDto> Documents { get; set; }
}

public class RequestDocumentDto
{
    public long Id { get; set; }
    public string DocumentName { get; set; }
    public string AccountType { get; set; }
    public DateTime UploadTime { get; set; }
    public string DocumentPath { get; set; }
    public string DocumentContent { get; set; }
}
