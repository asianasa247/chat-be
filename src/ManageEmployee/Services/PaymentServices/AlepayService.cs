using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Payments;
using ManageEmployee.Entities.PaymentEntities;
using ManageEmployee.Services.Interfaces.PaymentServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ManageEmployee.Services.PaymentServices
{
    public class AlepayService : IAlepayService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpClientFactory _http;
        private readonly ILogger<AlepayService> _logger;

        public AlepayService(ApplicationDbContext db, IHttpClientFactory http, ILogger<AlepayService> logger)
        {
            _db = db;
            _http = http;
            _logger = logger;
        }

        private async Task<AlepayConfig> GetActiveConfigAsync(int? additionWebId, CancellationToken ct)
        {
            var q = _db.Set<AlepayConfig>().AsNoTracking();
            var cfg = await q.FirstOrDefaultAsync(x => x.AdditionWebId == additionWebId, ct)
                   ?? await q.FirstOrDefaultAsync(x => x.AdditionWebId == null, ct)
                   ?? throw new InvalidOperationException("AlepayConfig chưa được thiết lập.");
            return cfg;
        }

        private static string BuildSignature(IDictionary<string, object?> data, string checksumKey)
        {
            var pairs = data.Where(kv => kv.Value is not null && kv.Key != "signature")
                            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                            .Select(kv => $"{kv.Key}={kv.Value}");
            var raw = string.Join("&", pairs);
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private static bool VerifyWebhookMd5(string orderCode, long amount, string transactionCode, string checksumKey, string provided)
        {
            var raw = $"{orderCode}{amount}{transactionCode}{checksumKey}";
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return string.Equals(hex, provided, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<AlepayCheckoutResponse> CreateCheckoutAsync(AlepayCheckoutRequest req, CancellationToken ct)
        {
            var cfg = await GetActiveConfigAsync(req.AdditionWebId, ct);
            var baseUrl = cfg.IsSandbox
                ? "https://alepay-v3-sandbox.nganluong.vn/api/v3/checkout"
                : "https://alepay-v3.nganluong.vn/api/v3/checkout";

            var tx = new PaymentTransaction
            {
                AdditionWebId = req.AdditionWebId,
                OrderCode = req.OrderCode,
                Amount = req.Amount,
                Currency = req.Currency,
                PaymentMethod = req.PaymentMethod ?? "",
                BankCode = req.BankCode,
                Installment = req.Installment,
                Month = req.Month,
                ReturnUrl = req.ReturnUrl,
                CancelUrl = req.CancelUrl,
                BuyerName = req.BuyerName,
                BuyerEmail = req.BuyerEmail,
                BuyerPhone = req.BuyerPhone,
                Status = "PENDING",
            };
            _db.Set<PaymentTransaction>().Add(tx);
            await _db.SaveChangesAsync(ct);

            var payload = new Dictionary<string, object?>
            {
                ["tokenKey"] = cfg.TokenKey,
                ["orderCode"] = req.OrderCode,
                ["amount"] = req.Amount,
                ["currency"] = req.Currency,
                ["orderDescription"] = req.OrderDescription,
                ["totalItem"] = 1,
                ["returnUrl"] = req.ReturnUrl,
                ["cancelUrl"] = req.CancelUrl,
                ["buyerName"] = req.BuyerName,
                ["buyerEmail"] = req.BuyerEmail,
                ["buyerPhone"] = req.BuyerPhone,
                ["paymentMethod"] = req.PaymentMethod,
                ["bankCode"] = req.BankCode,
                ["installment"] = req.Installment,
                ["month"] = req.Month
            };
            if (!string.IsNullOrWhiteSpace(req.CustomMerchantId)) payload["customMerchantId"] = req.CustomMerchantId;

            payload["signature"] = BuildSignature(payload, cfg.ChecksumKey);

            tx.ProviderPayload = JsonSerializer.Serialize(payload);

            var client = _http.CreateClient(nameof(AlepayService));
            var httpReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/request-payment")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            var resp = await client.SendAsync(httpReq, ct);
            var json = await resp.Content.ReadAsStringAsync(ct);
            tx.ProviderResponse = json;
            await _db.SaveChangesAsync(ct);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            string GetStr(string name) => root.TryGetProperty(name, out var v)
                ? (v.ValueKind == JsonValueKind.String ? v.GetString() : v.ToString()) ?? "" : "";

            var result = new AlepayCheckoutResponse
            {
                ErrorCode = GetStr("errorCode"),
                Message = GetStr("message"),
                CheckoutUrl = GetStr("checkoutUrl"),
                TransactionCode = GetStr("transactionCode"),
                Raw = json
            };

            if (!string.IsNullOrEmpty(result.TransactionCode))
            {
                tx.TransactionCode = result.TransactionCode!;
                await _db.SaveChangesAsync(ct);
            }

            return result;
        }

        public async Task<PaymentTransaction?> HandleReturnAsync(AlepayReturnQuery query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.TransactionCode)) return null;
            var tx = await SyncTransactionInfoAsync(query.TransactionCode!, ct);
            return tx;
        }

        public async Task<PaymentTransaction?> SyncTransactionInfoAsync(string transactionCode, CancellationToken ct)
        {
            var tx = await _db.Set<PaymentTransaction>().FirstOrDefaultAsync(x => x.TransactionCode == transactionCode, ct);
            if (tx == null) return null;

            var cfg = await GetActiveConfigAsync(tx.AdditionWebId, ct);
            var baseUrl = cfg.IsSandbox
                ? "https://alepay-v3-sandbox.nganluong.vn/api/v3/checkout"
                : "https://alepay-v3.nganluong.vn/api/v3/checkout";

            var data = new Dictionary<string, object?>
            {
                ["tokenKey"] = cfg.TokenKey,
                ["transactionCode"] = transactionCode
            };
            data["signature"] = BuildSignature(data, cfg.ChecksumKey);

            var client = _http.CreateClient(nameof(AlepayService));
            var httpReq = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/get-transaction-info")
            {
                Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            var resp = await client.SendAsync(httpReq, ct);
            var json = await resp.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("status", out var s)) tx.Status = s.GetString() ?? tx.Status;
            tx.ProviderResponse = json;
            await _db.SaveChangesAsync(ct);
            return tx;
        }

        public async Task<bool> HandleWebhookAsync(AlepayWebhookDto payload, CancellationToken ct)
        {
            var log = new PaymentWebhookLog { Raw = JsonSerializer.Serialize(payload), Verified = false };
            _db.Set<PaymentWebhookLog>().Add(log);

            if (string.IsNullOrWhiteSpace(payload.TransactionInfo))
            {
                log.Note = "Missing transactionInfo";
                await _db.SaveChangesAsync(ct);
                return false;
            }

            using var infoDoc = JsonDocument.Parse(payload.TransactionInfo!);
            var info = infoDoc.RootElement;

            var orderCode = info.GetProperty("orderCode").GetString() ?? "";
            var transactionCode = info.GetProperty("transactionCode").GetString() ?? "";
            var amount = info.GetProperty("amount").GetInt64();

            log.TransactionCode = transactionCode;
            log.OrderCode = orderCode;

            var tx = await _db.Set<PaymentTransaction>().FirstOrDefaultAsync(x => x.TransactionCode == transactionCode, ct);
            var cfg = await GetActiveConfigAsync(tx?.AdditionWebId, ct);

            var ok = VerifyWebhookMd5(orderCode, amount, transactionCode, cfg.ChecksumKey, payload.Checksum ?? "");
            log.Verified = ok;

            if (ok && tx != null)
            {
                tx.WebhookPayload = payload.TransactionInfo;
                if (info.TryGetProperty("status", out var s)) tx.Status = s.GetString() ?? tx.Status;
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                log.Note = "Checksum invalid or transaction not found";
            }

            await _db.SaveChangesAsync(ct);
            return ok;
        }

        public Task<PaymentTransaction?> GetByTransactionCodeAsync(string transactionCode, CancellationToken ct)
            => _db.Set<PaymentTransaction>().FirstOrDefaultAsync(x => x.TransactionCode == transactionCode, ct);
    }
}
