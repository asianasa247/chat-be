namespace ManageEmployee.DataTransferObject.Payments
{
    public class AlepayWebhookDto
    {
        public string? TransactionInfo { get; set; }
        public string? Checksum { get; set; }
    }
}
