using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Subscribers theo appCode.
    /// - Đọc đường dẫn từ: Zalo:Apps[*].SubscribersFile
    /// - Fallback: "Data/zalo/{appCode}.subscribers.json"
    /// - Backward compatible: nếu appCode trống -> Chatbot:SubscribersFile
    /// </summary>
    public sealed class FileSubscribersStore : ISubscribersStore
    {
        private readonly IConfiguration _cfg;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public FileSubscribersStore(IConfiguration cfg) => _cfg = cfg;

        public async Task<HashSet<string>> GetAllAsync(string appCode, CancellationToken ct = default)
        {
            var path = ResolvePath(appCode);
            if (!File.Exists(path)) return new();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var fs = File.OpenRead(path);
            var list = await JsonSerializer.DeserializeAsync<List<string>>(fs, _json, ct) ?? new();
            return list.ToHashSet();
        }

        public async Task AddAsync(string appCode, string userId, CancellationToken ct = default)
        {
            var set = await GetAllAsync(appCode, ct);
            if (set.Add(userId)) await SaveAsync(appCode, set, ct);
        }

        public async Task RemoveAsync(string appCode, string userId, CancellationToken ct = default)
        {
            var set = await GetAllAsync(appCode, ct);
            if (set.Remove(userId)) await SaveAsync(appCode, set, ct);
        }

        private async Task SaveAsync(string appCode, HashSet<string> set, CancellationToken ct)
        {
            var path = ResolvePath(appCode);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await using var fs = File.Create(path);
            await JsonSerializer.SerializeAsync(fs, set.ToList(), _json, ct);
        }

        private string ResolvePath(string appCode)
        {
            if (string.IsNullOrWhiteSpace(appCode))
                return _cfg["Chatbot:SubscribersFile"] ?? "Data/subscribers.json";

            var apps = _cfg.GetSection("Zalo:Apps").GetChildren();
            foreach (var a in apps)
            {
                if (string.Equals(a["Code"], appCode, StringComparison.OrdinalIgnoreCase))
                {
                    var file = a["SubscribersFile"];
                    if (!string.IsNullOrWhiteSpace(file)) return file!;
                }
            }
            return Path.Combine("Data", "zalo", $"{appCode}.subscribers.json");
        }
    }
}
