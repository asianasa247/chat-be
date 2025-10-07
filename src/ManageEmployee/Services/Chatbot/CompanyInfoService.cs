using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ManageEmployee.Entities.Chatbot;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    // Đọc companyInfo.json và trả lời nhanh các câu phổ biến (địa chỉ, hotline, email, tên)
    public sealed class CompanyInfoService : ICompanyInfoService
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
        private CompanyInfo? _cache;

        public CompanyInfoService(IConfiguration cfg)
        {
            _path = cfg["Chatbot:CompanyInfoPath"] ?? "Data/companyInfo.json";
        }

        public async Task<CompanyInfo> LoadAsync(CancellationToken ct = default)
        {
            if (_cache != null) return _cache;
            if (!File.Exists(_path)) return _cache = new CompanyInfo { Name = "Company" };

            await using var fs = File.OpenRead(_path);
            _cache = await JsonSerializer.DeserializeAsync<CompanyInfo>(fs, _json, ct) ?? new CompanyInfo { Name = "Company" };
            return _cache!;
        }

        public string? QuickAnswer(CompanyInfo info, string? userText)
        {
            if (string.IsNullOrWhiteSpace(userText)) return null;

            string Norm(string s)
            {
                s = s.ToLowerInvariant();
                s = Regex.Replace(s, @"\s+", " ").Trim();
                s = s.Normalize(NormalizationForm.FormD);
                s = new string(s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
                return s;
            }

            var q = Norm(userText);

            bool Hit(params string[] keys) => keys.Any(k => q.Contains(Norm(k)));

            if (!string.IsNullOrWhiteSpace(info.Address) && Hit("địa chỉ", "address", "ở đâu"))
                return $"Địa chỉ: {info.Address}";
            if (!string.IsNullOrWhiteSpace(info.Hotline) && Hit("hotline", "số điện thoại", "liên hệ"))
                return $"Hotline: {info.Hotline}";
            if (!string.IsNullOrWhiteSpace(info.Email) && Hit("email", "mail"))
                return $"Email: {info.Email}";
            if (!string.IsNullOrWhiteSpace(info.Name) && Hit("tên công ty", "company name"))
                return $"Tên công ty: {info.Name}";

            // Thử FAQ trong file (nếu có)
            foreach (var f in info.Faq)
            {
                if (q.Contains(Norm(f.Question)) || Norm(f.Question).Contains(q))
                    return f.Answer;
            }
            return null;
        }
    }
}
