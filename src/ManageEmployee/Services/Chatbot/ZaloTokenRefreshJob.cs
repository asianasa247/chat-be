using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.Extensions.DependencyInjection;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Job Hangfire: pre-warm/refresh token cho tất cả app trong Zalo:Apps.
    /// Backward compatible: nếu không có Apps -> chạy app "default" (cấu hình cũ).
    /// </summary>
    public sealed class ZaloTokenRefreshJob
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<ZaloTokenRefreshJob> _log;
        private readonly IConfiguration _cfg;

        public ZaloTokenRefreshJob(IServiceProvider sp, ILogger<ZaloTokenRefreshJob> log, IConfiguration cfg)
        {
            _sp = sp; _log = log; _cfg = cfg;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var api = scope.ServiceProvider.GetRequiredService<IZaloApiService>();

                var hasAny = false;
                foreach (var a in _cfg.GetSection("Zalo:Apps").GetChildren())
                {
                    var code = a["Code"];
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        hasAny = true;
                        await api.EnsureAccessTokenAsync(code!, ct);
                    }
                }

                // Fallback single-app
                if (!hasAny)
                {
                    await api.EnsureAccessTokenAsync("default", ct);
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "ZaloTokenRefreshJob run failed.");
            }
        }
    }
}
