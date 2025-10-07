using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.PaymentEntities
{
    public class PaymentTransaction : BaseEntity
    {
        public long Id { get; set; }
        public int? AdditionWebId { get; set; }

        [Required, StringLength(64)]
        public string OrderCode { get; set; } = "";
        public long Amount { get; set; }
        [StringLength(8)] public string Currency { get; set; } = "VND";

        [StringLength(64)] public string TransactionCode { get; set; } = "";
        [StringLength(32)] public string Status { get; set; } = "PENDING";

        [StringLength(32)] public string PaymentMethod { get; set; } = "";
        [StringLength(32)] public string? BankCode { get; set; }

        public bool Installment { get; set; }
        public int? Month { get; set; }

        [StringLength(256)] public string ReturnUrl { get; set; } = "";
        [StringLength(256)] public string CancelUrl { get; set; } = "";

        [StringLength(128)] public string? BuyerName { get; set; }
        [StringLength(128)] public string? BuyerEmail { get; set; }
        [StringLength(32)]  public string? BuyerPhone { get; set; }

        public string? ProviderPayload { get; set; }
        public string? ProviderResponse { get; set; }
        public string? WebhookPayload { get; set; }
    }
}
