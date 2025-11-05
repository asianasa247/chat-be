using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Job Hangfire: mỗi phút kiểm tra bảng ChatboxAIScheduledMessage và gửi tin.
    /// - NEW: chạy theo app dành cho "scheduler" (Zalo:SchedulerAppCode), 
    ///   nếu thiếu sẽ tìm app có Role="scheduler" trong Zalo:Apps.
    /// </summary>
    public sealed class ZaloSchedulePollingJob
    {
        private readonly ApplicationDbContext _db;
        private readonly IZaloApiService _zalo;
        private readonly ISubscribersStore _subs;
        private readonly ILogger<ZaloSchedulePollingJob> _log;
        private readonly IConfiguration _cfg;

        public ZaloSchedulePollingJob(
            ApplicationDbContext db,
            IZaloApiService zalo,
            ISubscribersStore subs,
            ILogger<ZaloSchedulePollingJob> log,
            IConfiguration cfg)
        {
            _db = db; _zalo = zalo; _subs = subs; _log = log; _cfg = cfg;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            var appCode = ResolveSchedulerAppCode();
            if (string.IsNullOrWhiteSpace(appCode))
            {
                _log.LogWarning("No scheduler app configured. Skip sending.");
                return;
            }

            var nowLocal = DateTime.Now;
            var slotStart = new DateTime(nowLocal.Year, nowLocal.Month, nowLocal.Day, nowLocal.Hour, nowLocal.Minute, 0, DateTimeKind.Local);
            var dow = nowLocal.DayOfWeek;

            var toSend = await _db.ChatboxAIScheduledMessages
                .AsNoTracking()
                .Include(x => x.Topic)
                .Where(x => x.SendTime.Hours == nowLocal.Hour && x.SendTime.Minutes == nowLocal.Minute)
                .ToListAsync(ct);

            if (toSend.Count == 0) return;

            var subs = await _subs.GetAllAsync(appCode, ct);
            if (subs.Count == 0) return;

            foreach (var msg in toSend)
            {
                if (!IsDayMatched(msg.DaysOfWeek, dow)) continue;

                var last = msg.LastSentAt?.ToLocalTime();
                if (last.HasValue && last.Value >= slotStart) continue;

                var text = string.IsNullOrWhiteSpace(msg.Message)
                    ? $"[{msg.Topic?.TopicName}] (Không có nội dung)"
                    : msg.Message;

                foreach (var uid in subs)
                {
                    try
                    {
                        await _zalo.SendTextAsync(appCode, uid, text, ct);
                        await Task.Delay(100, ct);
                    }
                    catch (Exception ex)
                    {
                        _log.LogWarning(ex, "Send scheduled message fail for {Uid}", uid);
                    }
                }

                var entity = await _db.ChatboxAIScheduledMessages.FirstOrDefaultAsync(x => x.Id == msg.Id, ct);
                if (entity != null)
                {
                    entity.LastSentAt = DateTime.UtcNow;
                    _db.ChatboxAIScheduledMessages.Update(entity);
                    await _db.SaveChangesAsync(ct);
                }
            }
        }

        private string ResolveSchedulerAppCode()
        {
            var code = _cfg["Zalo:SchedulerAppCode"];
            if (!string.IsNullOrWhiteSpace(code)) return code!;

            foreach (var a in _cfg.GetSection("Zalo:Apps").GetChildren())
            {
                if (string.Equals(a["Role"], "scheduler", StringComparison.OrdinalIgnoreCase))
                    return a["Code"] ?? "";
            }
            return "";
        }

        private static bool IsDayMatched(string? daysOfWeek, DayOfWeek today)
        {
            if (string.IsNullOrWhiteSpace(daysOfWeek)) return true;
            var s = daysOfWeek.Trim();
            if (s.Equals("Daily", StringComparison.OrdinalIgnoreCase)) return true;

            var tokens = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                          .Select(x => x.ToLowerInvariant()).ToList();

            var map = new Dictionary<string, DayOfWeek>(StringComparer.OrdinalIgnoreCase)
            {
                ["sun"] = DayOfWeek.Sunday,
                ["0"] = DayOfWeek.Sunday,
                ["mon"] = DayOfWeek.Monday,
                ["1"] = DayOfWeek.Monday,
                ["tue"] = DayOfWeek.Tuesday,
                ["2"] = DayOfWeek.Tuesday,
                ["wed"] = DayOfWeek.Wednesday,
                ["3"] = DayOfWeek.Wednesday,
                ["thu"] = DayOfWeek.Thursday,
                ["4"] = DayOfWeek.Thursday,
                ["fri"] = DayOfWeek.Friday,
                ["5"] = DayOfWeek.Friday,
                ["sat"] = DayOfWeek.Saturday,
                ["6"] = DayOfWeek.Saturday,
            };

            return tokens.Any(t =>
            {
                var key = t.Length >= 3 ? t.Substring(0, 3) : t;
                return map.TryGetValue(key, out var d) && d == today;
            });
        }
    }
}
