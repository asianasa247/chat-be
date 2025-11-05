using System.Text.Json;
using ManageEmployee.Entities.Chatbot;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Lưu token OA theo appCode.
    /// - Đọc đường dẫn từ: Zalo:Apps[*].TokensFile.
    /// - Fallback: "Data/zalo/{appCode}.tokens.json" nếu thiếu cấu hình.
    /// - Vẫn tương thích cấu hình cũ: nếu appCode trống -> dùng Chatbot:TokensFile.
    /// </summary>
    public sealed class FileTokenStore : ITokenStore
    {
        private readonly IConfiguration _cfg;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public FileTokenStore(IConfiguration cfg) => _cfg = cfg;

        public bool IsExpired(ZaloTokens t, int skewSeconds = 60)
            => DateTimeOffset.UtcNow >= t.ExpiresAt.AddSeconds(-skewSeconds);

        public async Task<ZaloTokens?> LoadAsync(string appCode, CancellationToken ct = default)
        {
            var path = ResolvePath(appCode);
            if (!File.Exists(path)) return null;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var fs = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<ZaloTokens>(fs, _json, ct);
        }

        public async Task SaveAsync(string appCode, ZaloTokens tokens, CancellationToken ct = default)
        {
            var path = ResolvePath(appCode);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, tokens, _json, ct);
        }

        private string ResolvePath(string appCode)
        {
            if (string.IsNullOrWhiteSpace(appCode))
                return _cfg["Chatbot:TokensFile"] ?? "Data/zalo_tokens.json";

            // Tìm theo Zalo:Apps
            var apps = _cfg.GetSection("Zalo:Apps").GetChildren();
            foreach (var a in apps)
            {
                if (string.Equals(a["Code"], appCode, StringComparison.OrdinalIgnoreCase))
                {
                    var file = a["TokensFile"];
                    if (!string.IsNullOrWhiteSpace(file)) return file!;
                }
            }
            // Fallback pattern
            return Path.Combine("Data", "zalo", $"{appCode}.tokens.json");
        }
    }
}
