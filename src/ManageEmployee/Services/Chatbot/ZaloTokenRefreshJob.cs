using ManageEmployee.Services.Chatbot;
using Microsoft.Extensions.DependencyInjection;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Job Hangfire gọi đảm bảo token luôn hợp lệ (gọi EnsureAccessTokenAsync gián tiếp).
    /// </summary>
    public sealed class ZaloTokenRefreshJob
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<ZaloTokenRefreshJob> _log;

        public ZaloTokenRefreshJob(IServiceProvider sp, ILogger<ZaloTokenRefreshJob> log)
        {
            _sp = sp; _log = log;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var api = scope.ServiceProvider.GetRequiredService<ZaloApiService>();

                // Hack nho nhỏ: gọi public SendTextAsync với userId giả? Không nên.
                // Thay vào đó, gọi EnsureAccessTokenAsync qua reflection để pre-warm.
                var mi = typeof(ZaloApiService).GetMethod("EnsureAccessTokenAsync",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (mi != null)
                {
                    var task = (Task)mi.Invoke(api, new object?[] { ct })!;
                    await task.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "ZaloTokenRefreshJob run failed.");
            }
        }
    }
}
