namespace ManageEmployee.DataTransferObject.Payments
{
    public class AlepayReturnQuery
    {
        public string? ErrorCode { get; set; }
        public string? TransactionCode { get; set; }
        public string? Cancel { get; set; }
    }
}
