using ManageEmployee.DataTransferObject.Payments;
using ManageEmployee.Entities.PaymentEntities;

namespace ManageEmployee.Services.Interfaces.PaymentServices
{
    public interface IAlepayService
    {
        Task<AlepayCheckoutResponse> CreateCheckoutAsync(AlepayCheckoutRequest req, CancellationToken ct);
        Task<PaymentTransaction?> HandleReturnAsync(AlepayReturnQuery query, CancellationToken ct);
        Task<bool> HandleWebhookAsync(AlepayWebhookDto payload, CancellationToken ct);

        Task<PaymentTransaction?> GetByTransactionCodeAsync(string transactionCode, CancellationToken ct);
        Task<PaymentTransaction?> SyncTransactionInfoAsync(string transactionCode, CancellationToken ct);
    }
}
