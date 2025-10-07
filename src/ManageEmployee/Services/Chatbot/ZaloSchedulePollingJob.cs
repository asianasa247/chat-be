using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Chatbot
{
    // Job Hangfire: mỗi phút kiểm tra bảng ChatboxAIScheduledMessage và gửi tin
    public sealed class ZaloSchedulePollingJob
    {
        private readonly ApplicationDbContext _db;
        private readonly IZaloApiService _zalo;
        private readonly ISubscribersStore _subs;
        private readonly ILogger<ZaloSchedulePollingJob> _log;

        public ZaloSchedulePollingJob(
            ApplicationDbContext db,
            IZaloApiService zalo,
            ISubscribersStore subs,
            ILogger<ZaloSchedulePollingJob> log)
        {
            _db = db; _zalo = zalo; _subs = subs; _log = log;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            var nowLocal = DateTime.Now; // server local time
            var slotStart = new DateTime(nowLocal.Year, nowLocal.Month, nowLocal.Day, nowLocal.Hour, nowLocal.Minute, 0, DateTimeKind.Local);
            var dow = nowLocal.DayOfWeek; // Sunday=0, Monday=1...

            // Lọc các bản ghi khớp phút hiện tại
            var toSend = await _db.ChatboxAIScheduledMessages
                .AsNoTracking() // đọc nhẹ
                .Include(x => x.Topic)
                .Where(x => x.SendTime.Hours == nowLocal.Hour && x.SendTime.Minutes == nowLocal.Minute)
                .ToListAsync(ct);

            if (toSend.Count == 0) return;

            // Tải subscribers
            var subs = await _subs.GetAllAsync(ct);
            if (subs.Count == 0) return;

            foreach (var msg in toSend)
            {
                // Kiểm tra ngày trong tuần
                if (!IsDayMatched(msg.DaysOfWeek, dow)) continue;

                // Chặn gửi trùng trong cùng 1 phút (dựa vào LastSentAt)
                var last = msg.LastSentAt?.ToLocalTime();
                if (last.HasValue && last.Value >= slotStart) continue;

                var text = string.IsNullOrWhiteSpace(msg.Message)
                    ? $"[{msg.Topic?.TopicName}] (Không có nội dung)"
                    : msg.Message;

                foreach (var uid in subs)
                {
                    try
                    {
                        await _zalo.SendTextAsync(uid, text, ct);
                        await Task.Delay(100, ct); // tránh rate limit
                    }
                    catch (Exception ex)
                    {
                        _log.LogWarning(ex, "Send scheduled message fail for {Uid}", uid);
                    }
                }

                // Ghi lại LastSentAt
                var entity = await _db.ChatboxAIScheduledMessages.FirstOrDefaultAsync(x => x.Id == msg.Id, ct);
                if (entity != null)
                {
                    entity.LastSentAt = DateTime.UtcNow;
                    _db.ChatboxAIScheduledMessages.Update(entity);
                    await _db.SaveChangesAsync(ct);
                }
            }
        }

        private static bool IsDayMatched(string? daysOfWeek, DayOfWeek today)
        {
            if (string.IsNullOrWhiteSpace(daysOfWeek)) return true;
            var s = daysOfWeek.Trim();
            if (s.Equals("Daily", StringComparison.OrdinalIgnoreCase)) return true;

            // Hỗ trợ cả tên (Mon,Tue,...) và số (0..6)
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

            return tokens.Any(t => map.TryGetValue(t.Substring(0, Math.Min(3, t.Length)), out var d) ? d == today : false);
        }
    }
}
