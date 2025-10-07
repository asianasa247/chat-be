namespace ManageEmployee.DataTransferObject.Payments
{
    public class AlepayCheckoutResponse
    {
        public string ErrorCode { get; set; } = "";
        public string? Message { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? TransactionCode { get; set; }
        public string? Raw { get; set; }
    }
}
