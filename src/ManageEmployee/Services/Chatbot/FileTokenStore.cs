using System.Text.Json;
using ManageEmployee.Entities.Chatbot;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    // Lưu token OA ở file JSON: Chatbot:TokensFile
    public sealed class FileTokenStore : ITokenStore
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public FileTokenStore(IConfiguration cfg)
        {
            _path = cfg["Chatbot:TokensFile"] ?? "Data/zalo_tokens.json";
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        }

        public bool IsExpired(ZaloTokens t, int skewSeconds = 60)
            => DateTimeOffset.UtcNow >= t.ExpiresAt.AddSeconds(-skewSeconds);

        public async Task<ZaloTokens?> LoadAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_path)) return null;
            await using var fs = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<ZaloTokens>(fs, _json, ct);
        }

        public async Task SaveAsync(ZaloTokens tokens, CancellationToken ct = default)
        {
            await using var fs = File.Create(_path);
            await JsonSerializer.SerializeAsync(fs, tokens, _json, ct);
        }
    }
}
