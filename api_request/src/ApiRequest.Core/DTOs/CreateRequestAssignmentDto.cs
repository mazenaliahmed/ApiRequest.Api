public class CreateRequestAssignmentDto
{
    public long RequestID { get; set; }
    public long ServiceProviderID { get; set; }

    // Optional - if not provided, default "PaymentPending" will be used.
    //public string? ExecutionStatus { get; set; }

    public string? Note { get; set; }

    // Numeric(18,0) maps to decimal in C#
    public decimal? Amount { get; set; }
}
