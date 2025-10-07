using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.Payments
{
    public class AlepayCheckoutRequest
    {
        [Required] public string OrderCode { get; set; } = "";
        [Required] public long Amount { get; set; }
        public string Currency { get; set; } = "VND";
        [Required] public string OrderDescription { get; set; } = "";

        public int? AdditionWebId { get; set; }

        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
        public string? BuyerPhone { get; set; }

        public string? PaymentMethod { get; set; }
        public string? BankCode { get; set; }
        public bool Installment { get; set; } = false;
        public int? Month { get; set; }

        [Required] public string ReturnUrl { get; set; } = "";
        [Required] public string CancelUrl { get; set; } = "";

        public string? CustomMerchantId { get; set; }
    }
}
