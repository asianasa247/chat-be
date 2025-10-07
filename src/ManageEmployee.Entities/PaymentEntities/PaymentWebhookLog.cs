using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.PaymentEntities
{
    public class PaymentWebhookLog : BaseEntity
    {
        public long Id { get; set; }
        public string Event { get; set; } = "alepay:webhook";
        public string? Raw { get; set; }
        public bool Verified { get; set; }
        public string? Note { get; set; }
        public string? TransactionCode { get; set; }
        public string? OrderCode { get; set; }
    }
}
