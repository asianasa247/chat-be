using ManageEmployee.Entities.Chatbot;

namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface ITokenStore
    {
        // NEW: đa-app theo appCode
        Task<ZaloTokens?> LoadAsync(string appCode, CancellationToken ct = default);
        Task SaveAsync(string appCode, ZaloTokens tokens, CancellationToken ct = default);
        bool IsExpired(ZaloTokens tokens, int skewSeconds = 60);
    }
}
