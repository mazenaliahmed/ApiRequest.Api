namespace ApiRequest.Core.Entities
{
    public class RequestDocument
    {
        public long Id { get; set; }
        public string DocumentName { get; set; }
        public long RequestId { get; set; }
        public string AccountType { get; set; }
        public DateTime UploadTime { get; set; }
        public string DocumentContent { get; set; }

        // Navigation properties
        public Request Request { get; set; }
        public string DocumentPath { get; set; }  // الحقل الذي سيحتوي على المسار الفيزيائي أو الافتراضي

    }
}
