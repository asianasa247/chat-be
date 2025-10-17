using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ManageEmployee.Entities.Chatbot;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    // Đọc companyInfo.json và trả lời nhanh các câu phổ biến (địa chỉ, hotline, email, tên, FAQ trigger)
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

            static string Norm(string s)
            {
                s = s.ToLowerInvariant();
                s = Regex.Replace(s, @"\s+", " ").Trim();
                s = s.Normalize(NormalizationForm.FormD);
                s = new string(s.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                                            != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray());
                return s;
            }

            var q = Norm(userText);

            bool Hit(params string[] keys) => keys.Any(k => q.Contains(Norm(k)));

            // Các câu hỏi nhanh (ưu tiên cao)
            if (!string.IsNullOrWhiteSpace(info.Address) && Hit("địa chỉ", "address", "ở đâu", "văn phòng"))
                return $"Địa chỉ: {info.Address}";

            if (!string.IsNullOrWhiteSpace(info.Hotline) && Hit("hotline", "số điện thoại", "liên hệ", "contact", "call"))
                return $"Hotline: {info.Hotline}";

            if (!string.IsNullOrWhiteSpace(info.Email) && Hit("email", "mail", "liên hệ"))
                return $"Email: {info.Email}";

            if (!string.IsNullOrWhiteSpace(info.Name) && Hit("tên công ty", "tên doanh nghiệp", "company name", "bên bạn tên gì"))
                return $"Tên công ty: {info.Name}";

            if (!string.IsNullOrWhiteSpace(info.WorkingHours) && Hit("giờ làm", "thời gian làm việc", "mở cửa"))
                return $"Giờ làm việc: {info.WorkingHours}";

            if (info.Services?.Count > 0 && Hit("dịch vụ", "cung cấp gì"))
                return $"Dịch vụ: {string.Join("; ", info.Services)}";

            // Match FAQ trong file companyInfo.json
            foreach (var f in info.Faq ?? new())
            {
                foreach (var trigger in f.Triggers ?? new())
                {
                    if (string.IsNullOrWhiteSpace(trigger)) continue;
                    var t = Norm(trigger);
                    if (q.Contains(t) || t.Contains(q))
                        return f.Answer;
                }
            }

            return null;
        }
    }
}
