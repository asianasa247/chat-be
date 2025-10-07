using System.Text;
using System.Text.RegularExpressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Chatbot
{
    // Core: 1) Company quick answer -> 2) ChatboxAIQA -> 3) Gemini (free)
    public sealed class ZaloChatbotService : IZaloChatbotService
    {
        private readonly ICompanyInfoService _company;
        private readonly IGeminiNlpService _gemini;
        private readonly IConfiguration _cfg;
        private readonly ApplicationDbContext _db;

        public ZaloChatbotService(
            ICompanyInfoService company,
            IGeminiNlpService gemini,
            IConfiguration cfg,
            ApplicationDbContext db)
        {
            _company = company;
            _gemini = gemini;
            _cfg = cfg;
            _db = db;
        }

        private static string Normalize(string s)
        {
            s = s.ToLowerInvariant();
            s = Regex.Replace(s, @"\s+", " ").Trim();
            s = s.Normalize(NormalizationForm.FormD);
            s = new string(s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
            return s;
        }

        public async Task<string> BuildReplyAsync(string userText, CancellationToken ct = default)
        {
            var prefix = _cfg["Chatbot:AutoPrefix"] ?? "";
            var info = await _company.LoadAsync(ct);

            // 1) Trả lời nhanh từ company info
            var quick = _company.QuickAnswer(info, userText);
            if (!string.IsNullOrWhiteSpace(quick)) return prefix + quick;

            // 2) Tra trong bảng ChatboxAIQA (accent-insensitive, match đơn giản)
            var normQ = Normalize(userText ?? "");
            var qas = await _db.ChatboxAIQAs
                .AsNoTracking()
                .Select(x => new { x.Question, x.Answer, x.TopicId, x.Id })
                .ToListAsync(ct);

            string? best = null;
            int bestScore = 0;
            foreach (var qa in qas)
            {
                var q = Normalize(qa.Question);
                var a = Normalize(qa.Answer);
                var score = 0;
                if (q.Contains(normQ)) score += 4;
                if (normQ.Contains(q)) score += 3;
                if (!score.Equals(0) && a.Contains(normQ)) score += 1;

                // heuristic theo số từ trùng
                var words = normQ.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct();
                score += words.Count(w => q.Contains(w));

                if (score > bestScore)
                {
                    bestScore = score;
                    best = qa.Answer;
                }
            }
            if (!string.IsNullOrWhiteSpace(best)) return prefix + best;

            // 3) Gemini (free) – thêm context công ty vào prompt cho gợi ý tốt hơn
            var ctx = $"Tên: {info.Name}\nĐịa chỉ: {info.Address}\nHotline: {info.Hotline}\nEmail: {info.Email}";
            var ai = await _gemini.GenerateAsync(
                $"Bạn là CSKH của công ty dưới đây, trả lời ngắn gọn, lịch sự, tiếng Việt. " +
                $"Nếu không chắc, hãy nói sẽ chuyển bộ phận hỗ trợ.\n--THÔNG TIN CÔNG TY--\n{ctx}\n--CÂU HỎI--\n{userText}", ct)
                     ?? "Xin lỗi, hiện mình chưa có câu trả lời phù hợp.";
            return prefix + ai;
        }
    }
}
