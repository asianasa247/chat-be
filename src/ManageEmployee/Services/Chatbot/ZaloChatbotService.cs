using System.Text;
using System.Text.RegularExpressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Flow chính:
    ///  - Nếu chưa có state hoặc user gõ "menu/chủ đề/bắt đầu/start/help" => hiển thị MENU CHỦ ĐỀ (từ ChatboxAITopics)
    ///  - awaiting_topic:
    ///     + Người dùng gõ SỐ (1..n) hoặc TÊN chủ đề => chuyển sang awaiting_question + hiển thị danh sách câu hỏi (ChatboxAIQAs)
    ///  - awaiting_question:
    ///     + Người dùng gõ SỐ (1..n) hoặc trích nội dung => trả lời Answer tương ứng
    ///  - Ở mọi nơi: nếu không khớp lựa chọn => thử fallback theo companyInfo.json; nếu vẫn không có => nhắc lại cách gõ.
    /// </summary>
    public sealed class ZaloChatbotService : IZaloChatbotService
    {
        private readonly ICompanyInfoService _company;
        private readonly IConfiguration _cfg;
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ZaloChatbotService> _log;

        public ZaloChatbotService(
            ICompanyInfoService company,
            IConfiguration cfg,
            ApplicationDbContext db,
            IMemoryCache cache,
            ILogger<ZaloChatbotService> log)
        {
            _company = company;
            _cfg = cfg;
            _db = db;
            _cache = cache;
            _log = log;
        }

        private enum Stage { AwaitingTopic, AwaitingQuestion }

        private sealed class TopicItem
        {
            public int Id { get; init; }
            public string Name { get; init; } = "";
        }

        private sealed class QaItem
        {
            public int Id { get; init; }
            public string Question { get; init; } = "";
            public string Answer { get; init; } = "";
        }

        private sealed class UserState
        {
            public Stage Stage { get; set; } = Stage.AwaitingTopic;
            public int? TopicId { get; set; }
            public string? TopicName { get; set; }
            public List<TopicItem> Topics { get; set; } = new();
            public List<QaItem> QAs { get; set; } = new();
            public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        }

        private static string CacheKey(string userId) => "chatbot.state:" + userId;

        private void SaveState(string userId, UserState st)
        {
            st.UpdatedAt = DateTimeOffset.UtcNow;
            _cache.Set(CacheKey(userId), st, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
            });
        }

        private bool TryGetState(string userId, out UserState st)
            => _cache.TryGetValue(CacheKey(userId), out st!);

        private static string Normalize(string s)
        {
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"\s+", " ").Trim();
            s = s.Normalize(NormalizationForm.FormD);
            s = new string(s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
            return s;
        }

        private static bool IsMenuTrigger(string textNorm)
        {
            var t = textNorm;
            return t.Contains("menu") ||
                   t.Contains("chu de") ||
                   t.Contains("chude") ||
                   t.Contains("bat dau") ||
                   t.Contains("batdau") ||
                   t.Contains("start") ||
                   t.Contains("help") ||
                   t == "?" || t == "bắt đầu" || t == "chủ đề";
        }

        private static bool TryPickNumberStrict(string raw, out int index)
        {
            index = 0;
            var trimmed = raw.Trim();
            if (!Regex.IsMatch(trimmed, @"^\d{1,3}(\.|:|\)|\s)?$")) return false;
            var m = Regex.Match(trimmed, @"\d+");
            if (!m.Success) return false;
            if (!int.TryParse(m.Value, out index)) return false;
            return true;
        }

        private async Task<List<TopicItem>> LoadTopicsAsync(CancellationToken ct)
        {
            var list = await _db.ChatboxAITopics
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new TopicItem { Id = x.Id, Name = x.TopicName })
                .ToListAsync(ct);

            return list;
        }

        private async Task<List<QaItem>> LoadQAsAsync(int topicId, CancellationToken ct)
        {
            var list = await _db.ChatboxAIQAs
                .AsNoTracking()
                .Where(x => x.TopicId == topicId)
                .OrderBy(x => x.Id)
                .Select(x => new QaItem { Id = x.Id, Question = x.Question, Answer = x.Answer })
                .ToListAsync(ct);

            return list;
        }

        private string BuildTopicMenu(List<TopicItem> topics)
        {
            var sb = new StringBuilder();
            sb.AppendLine("✨AI của JW Kim thông minh nhất😄");
            sb.AppendLine();
            sb.AppendLine("Mình chưa nhận ra chủ đề bạn chọn. Vui lòng gõ **số** hoặc **tên** chủ đề.");
            sb.AppendLine();

            if (topics.Count == 0)
            {
                sb.AppendLine("_Hiện chưa có chủ đề nào. Vui lòng quay lại sau._");
                return WithPrefix(sb.ToString());
            }

            for (int i = 0; i < topics.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {topics[i].Name}");
            }

            sb.AppendLine();
            sb.AppendLine("💞Vui lòng \"Gõ số hoặc tên\" nhé:");
            return WithPrefix(sb.ToString());
        }

        private string BuildQaMenu(string topicName, List<QaItem> qas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Đây là tin nhắn tự động của chatbot.");
            sb.AppendLine();
            sb.AppendLine($"Chủ đề: **{topicName}**");
            sb.AppendLine("Chọn **Câu hỏi** (gõ số hoặc trích nội dung):");
            sb.AppendLine();

            if (qas.Count == 0)
            {
                sb.AppendLine("_Chủ đề này hiện chưa có câu hỏi nào._");
                sb.AppendLine();
                sb.AppendLine("Gõ **menu** để chọn chủ đề khác.");
                return WithPrefix(sb.ToString());
            }

            for (int i = 0; i < qas.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {qas[i].Question}");
            }
            return WithPrefix(sb.ToString());
        }

        private string WithPrefix(string msg)
        {
            var prefix = _cfg["Chatbot:AutoPrefix"] ?? "";
            return string.IsNullOrEmpty(prefix) ? msg : prefix + msg;
        }

        public async Task<string> BuildReplyAsync(string userId, string userText, CancellationToken ct = default)
        {
            userText ??= "";
            var textNorm = Normalize(userText);

            // Lấy thông tin công ty (phục vụ fallback nhanh)
            var company = await _company.LoadAsync(ct);

            // Trường hợp người dùng yêu cầu MENU / bắt đầu lại
            if (IsMenuTrigger(textNorm) || !TryGetState(userId, out var state))
            {
                var topics = await LoadTopicsAsync(ct);
                var fresh = new UserState
                {
                    Stage = Stage.AwaitingTopic,
                    Topics = topics,
                    TopicId = null,
                    TopicName = null,
                    QAs = new()
                };
                SaveState(userId, fresh);
                return BuildTopicMenu(topics);
            }

            // ====== ĐANG Ở TRẠNG THÁI CHỌN CHỦ ĐỀ ======
            if (state.Stage == Stage.AwaitingTopic)
            {
                // Đảm bảo đã có danh sách topics
                if (state.Topics.Count == 0)
                    state.Topics = await LoadTopicsAsync(ct);

                // Thử match theo SỐ
                if (TryPickNumberStrict(userText, out var idx))
                {
                    if (idx >= 1 && idx <= state.Topics.Count)
                    {
                        var pick = state.Topics[idx - 1];
                        state.TopicId = pick.Id;
                        state.TopicName = pick.Name;
                        state.QAs = await LoadQAsAsync(pick.Id, ct);
                        state.Stage = Stage.AwaitingQuestion;
                        SaveState(userId, state);
                        return BuildQaMenu(pick.Name, state.QAs);
                    }
                }

                // Thử match theo TÊN (accent-insensitive)
                var match = state.Topics.FirstOrDefault(t =>
                {
                    var tn = Normalize(t.Name);
                    return tn.Contains(textNorm) || textNorm.Contains(tn);
                });

                if (match != null)
                {
                    state.TopicId = match.Id;
                    state.TopicName = match.Name;
                    state.QAs = await LoadQAsAsync(match.Id, ct);
                    state.Stage = Stage.AwaitingQuestion;
                    SaveState(userId, state);
                    return BuildQaMenu(match.Name, state.QAs);
                }

                // Không chọn được => fallback nhanh theo company info?
                var quick = _company.QuickAnswer(company, userText);
                if (!string.IsNullOrWhiteSpace(quick))
                {
                    // Giữ nguyên stage và nhắc lại menu để user chọn tiếp
                    var ans = new StringBuilder();
                    ans.AppendLine(quick);
                    ans.AppendLine();
                    ans.Append(BuildTopicMenu(state.Topics));
                    return WithPrefix(ans.ToString());
                }

                // Nhắc lại menu chủ đề
                SaveState(userId, state);
                return BuildTopicMenu(state.Topics);
            }

            // ====== ĐANG Ở TRẠNG THÁI CHỌN CÂU HỎI ======
            if (state.Stage == Stage.AwaitingQuestion && state.TopicId.HasValue)
            {
                // Bảo toàn list câu hỏi
                if (state.QAs.Count == 0)
                    state.QAs = await LoadQAsAsync(state.TopicId.Value, ct);

                // SỐ
                if (TryPickNumberStrict(userText, out var idx))
                {
                    if (idx >= 1 && idx <= state.QAs.Count)
                    {
                        var qa = state.QAs[idx - 1];
                        SaveState(userId, state); // giữ state để hỏi thêm
                        return WithPrefix(qa.Answer);
                    }
                }

                // TRÍCH NỘI DUNG
                var pick = state.QAs.FirstOrDefault(q =>
                {
                    var qn = Normalize(q.Question);
                    return qn.Contains(textNorm) || textNorm.Contains(qn);
                });

                if (pick != null)
                {
                    SaveState(userId, state);
                    return WithPrefix(pick.Answer);
                }

                // Fallback theo companyInfo.json
                var quick = _company.QuickAnswer(company, userText);
                if (!string.IsNullOrWhiteSpace(quick))
                {
                    SaveState(userId, state);
                    return WithPrefix(quick);
                }

                // Không khớp gì => nhắc lại menu câu hỏi
                SaveState(userId, state);
                return BuildQaMenu(state.TopicName ?? "Chủ đề", state.QAs);
            }

            // Nếu vì lý do gì state lệch, reset về topic menu
            var topicsReset = await LoadTopicsAsync(ct);
            var reset = new UserState { Stage = Stage.AwaitingTopic, Topics = topicsReset };
            SaveState(userId, reset);
            return BuildTopicMenu(topicsReset);
        }
    }
}
