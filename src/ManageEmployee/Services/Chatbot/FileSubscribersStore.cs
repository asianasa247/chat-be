using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    // Lưu subscribers vào file JSON: Chatbot:SubscribersFile
    public sealed class FileSubscribersStore : ISubscribersStore
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public FileSubscribersStore(IConfiguration cfg)
        {
            _path = cfg["Chatbot:SubscribersFile"] ?? "Data/subscribers.json";
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        }

        public async Task<HashSet<string>> GetAllAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_path)) return new();
            await using var fs = File.OpenRead(_path);
            var list = await JsonSerializer.DeserializeAsync<List<string>>(fs, _json, ct) ?? new();
            return list.ToHashSet();
        }

        public async Task AddAsync(string userId, CancellationToken ct = default)
        {
            var set = await GetAllAsync(ct);
            if (set.Add(userId))
                await SaveAsync(set, ct);
        }

        public async Task RemoveAsync(string userId, CancellationToken ct = default)
        {
            var set = await GetAllAsync(ct);
            if (set.Remove(userId))
                await SaveAsync(set, ct);
        }

        private async Task SaveAsync(HashSet<string> set, CancellationToken ct)
        {
            await using var fs = File.Create(_path);
            await JsonSerializer.SerializeAsync(fs, set.ToList(), _json, ct);
        }
    }
}
