using System.Text;
using System.Text.RegularExpressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ManageEmployee.Services.Chatbot
{
    /// Flow mới:
    /// - Luôn có "chọn chủ đề" / "chọn lại chủ đề" / "menu|start|help" để quay về Topic Menu.
    /// - Khi chọn 1 chủ đề: hiển thị danh sách câu hỏi (QA) của chủ đề đó.
    /// - Khi chọn câu hỏi: trả lời Answer + hiển thị lại danh sách câu hỏi.
    /// - Fallback: nếu không hiểu, ưu tiên trả lời nhanh từ CompanyInfo; nếu vẫn không, nhắc lại menu tương ứng.
    /// - Hoàn toàn bỏ "[AUTO]" khỏi mọi message.
    public sealed class ZaloChatbotService : IZaloChatbotService
    {
        private readonly ICompanyInfoService _company;
        private readonly IGeminiNlpService _gemini; // hiện không dùng nhưng giữ DI cho ổn định
        private readonly IConfiguration _cfg;
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;

        public ZaloChatbotService(
            ICompanyInfoService company,
            IGeminiNlpService gemini,
            IConfiguration cfg,
            ApplicationDbContext db,
            IMemoryCache cache)
        {
            _company = company;
            _gemini = gemini;
            _cfg = cfg;
            _db = db;
            _cache = cache;
        }

        // ==== State trong cache ====
        private record IdName(int Id, string Name);
        private sealed class ChatState
        {
            public int? TopicId { get; set; }
            public string? TopicName { get; set; }
            public List<IdName> CachedTopics { get; set; } = new();
            public List<IdName> CachedQuestions { get; set; } = new();
            public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        }

        private ChatState GetState(string userId)
            => _cache.GetOrCreate($"zalo.chat.state.{userId}", e =>
            {
                e.SlidingExpiration = TimeSpan.FromMinutes(30);
                return new ChatState();
            })!;

        private void SaveState(string userId, ChatState st)
        {
            st.UpdatedAt = DateTimeOffset.UtcNow;
            _cache.Set($"zalo.chat.state.{userId}", st, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });
        }

        // ==== Utils ====
        private static string Normalize(string s)
        {
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"\s+", " ").Trim();
            s = s.Normalize(NormalizationForm.FormD);
            s = new string(s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
            return s;
        }

        private static bool IsBackToTopics(string norm)
        {
            // các lệnh về Menu
            return norm is "menu" or "start" or "help"
                || norm.Contains("chon chu de")
                || norm.Contains("chọn chủ đề")
                || norm.Contains("chon lai chu de")
                || norm.Contains("chọn lại chủ đề")
                || norm.Contains("quay lai") || norm.Contains("quay lại");
        }

        // ==== Build menu text ====
        private async Task<(string Text, List<IdName> Topics)> BuildTopicMenuAsync(CancellationToken ct)
        {
            var topics = await _db.ChatboxAITopics
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new IdName(x.Id, x.TopicName))
                .ToListAsync(ct);

            var sb = new StringBuilder();
            sb.AppendLine("✨AI của JW Kim thông minh nhất😄");
            sb.AppendLine();
            sb.AppendLine("Mình chưa nhận ra chủ đề bạn chọn. Vui lòng gõ **số** hoặc **tên** chủ đề.");
            sb.AppendLine();

            for (int i = 0; i < topics.Count; i++)
                sb.AppendLine($"{i + 1}. {topics[i].Name}");

            sb.AppendLine();
            sb.Append("💞Vui lòng \"Gõ số hoặc tên\" nhé:");
            return (sb.ToString(), topics);
        }

        private async Task<(string Text, List<IdName> Questions, string TopicName)> BuildQuestionMenuAsync(int topicId, CancellationToken ct)
        {
            var topic = await _db.ChatboxAITopics.AsNoTracking().FirstOrDefaultAsync(x => x.Id == topicId, ct);
            var tName = topic?.TopicName ?? "Chủ đề";

            var qas = await _db.ChatboxAIQAs
                .AsNoTracking()
                .Where(x => x.TopicId == topicId)
                .OrderBy(x => x.Id)
                .Select(x => new IdName(x.Id, x.Question))
                .ToListAsync(ct);

            var sb = new StringBuilder();
            sb.AppendLine("Đây là tin nhắn tự động của chatbot.");
            sb.AppendLine();
            sb.AppendLine($"Chủ đề: **{tName}**");
            sb.AppendLine("Chọn **Câu hỏi** (gõ số hoặc trích nội dung):");
            sb.AppendLine();

            for (int i = 0; i < qas.Count; i++)
                sb.AppendLine($"{i + 1}. {qas[i].Name}");

            sb.AppendLine();
            sb.Append("Nếu muốn chọn chủ đề khác, vui lòng gõ \"chọn chủ đề\".");

            return (sb.ToString(), qas, tName);
        }

        // ==== Parse selection helpers ====
        private static int? TryParseIndex(string norm)
        {
            // nếu người dùng chỉ gõ số
            if (int.TryParse(norm, out var n) && n > 0) return n;
            // nếu có dạng "1." hoặc "1 -"...
            var m = Regex.Match(norm, @"^(\d+)[\.\-\)]");
            if (m.Success && int.TryParse(m.Groups[1].Value, out var k) && k > 0) return k;
            return null;
        }

        private static int? MatchByIndexOrName(string inputNorm, List<IdName> items)
        {
            // 1) thử theo số thứ tự
            var idx = TryParseIndex(inputNorm);
            if (idx.HasValue && idx.Value <= items.Count) return items[idx.Value - 1].Id;

            // 2) theo tên (chứa)
            var match = items.FirstOrDefault(it => Normalize(it.Name).Contains(inputNorm) || inputNorm.Contains(Normalize(it.Name)));
            return match?.Id;
        }

        // ==== Core ====
        public async Task<string> BuildReplyAsync(string userId, string userText, CancellationToken ct = default)
        {
            var st = GetState(userId);
            var info = await _company.LoadAsync(ct);
            var norm = Normalize(userText ?? "");

            // Lệnh quay về menu chủ đề
            if (IsBackToTopics(norm))
            {
                st.TopicId = null;
                st.TopicName = null;
                var (menu, topics) = await BuildTopicMenuAsync(ct);
                st.CachedTopics = topics;
                st.CachedQuestions = new();
                SaveState(userId, st);
                return menu;
            }

            // === Nếu chưa chọn chủ đề → hiển thị & chọn chủ đề ===
            if (st.TopicId is null)
            {
                // nếu chưa cache, build menu
                if (st.CachedTopics.Count == 0)
                {
                    var (menu, topics) = await BuildTopicMenuAsync(ct);
                    st.CachedTopics = topics;
                    SaveState(userId, st);

                    // nếu userText gõ gì đó không map được, trả luôn menu
                    var chosenTopicId = MatchByIndexOrName(norm, topics);
                    if (chosenTopicId is null) return menu;

                    // chọn được topic ngay lần đầu
                    st.TopicId = chosenTopicId;
                    var (qMenu, qList, tName) = await BuildQuestionMenuAsync(st.TopicId.Value, ct);
                    st.TopicName = tName;
                    st.CachedQuestions = qList;
                    SaveState(userId, st);
                    return qMenu;
                }
                else
                {
                    // đã có cache topics → cố gắng map lựa chọn
                    var chosenTopicId = MatchByIndexOrName(norm, st.CachedTopics);
                    if (chosenTopicId is null)
                    {
                        var (menu, topics) = await BuildTopicMenuAsync(ct);
                        st.CachedTopics = topics;
                        SaveState(userId, st);
                        return menu;
                    }

                    st.TopicId = chosenTopicId;
                    var (qMenu, qList, tName) = await BuildQuestionMenuAsync(st.TopicId.Value, ct);
                    st.TopicName = tName;
                    st.CachedQuestions = qList;
                    SaveState(userId, st);
                    return qMenu;
                }
            }

            // === Đã có chủ đề → chọn câu hỏi & trả lời ===
            // Cố gắng đồng bộ danh sách câu hỏi
            if (st.CachedQuestions.Count == 0)
            {
                var (_, qList, tName) = await BuildQuestionMenuAsync(st.TopicId!.Value, ct);
                st.TopicName = tName;
                st.CachedQuestions = qList;
                SaveState(userId, st);
            }

            var chosenQaId = MatchByIndexOrName(norm, st.CachedQuestions);
            if (chosenQaId is not null)
            {
                var qa = await _db.ChatboxAIQAs.AsNoTracking()
                    .Where(x => x.Id == chosenQaId.Value)
                    .Select(x => new { x.Question, x.Answer, x.TopicId })
                    .FirstOrDefaultAsync(ct);

                if (qa is not null)
                {
                    // Sau khi trả lời, hiển thị lại menu câu hỏi
                    var (qMenu, qList, tName) = await BuildQuestionMenuAsync(qa.TopicId, ct);
                    st.TopicId = qa.TopicId;
                    st.TopicName = tName;
                    st.CachedQuestions = qList;
                    SaveState(userId, st);

                    var sb = new StringBuilder();
                    sb.AppendLine(qa.Answer?.Trim() ?? "");
                    sb.AppendLine();
                    sb.Append(qMenu);
                    return sb.ToString();
                }
            }

            // Fallback trong phạm vi chủ đề: thử trả lời nhanh từ CompanyInfo
            var quick = _company.QuickAnswer(info, userText);
            if (!string.IsNullOrWhiteSpace(quick))
            {
                var (qMenu, _, _) = await BuildQuestionMenuAsync(st.TopicId!.Value, ct);
                return quick + "\n\n" + qMenu;
            }

            // Không nhận ra: nhắc lại danh sách câu hỏi & hướng dẫn chọn chủ đề
            {
                var (qMenu, qList, tName) = await BuildQuestionMenuAsync(st.TopicId!.Value, ct);
                st.TopicName = tName;
                st.CachedQuestions = qList;
                SaveState(userId, st);
                return "Mình chưa nhận ra câu hỏi. Bạn vui lòng **gõ số** hoặc **trích nội dung** của câu hỏi nhé.\n\n" + qMenu;
            }
        }
    }
}
