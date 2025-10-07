using ManageEmployee.Entities.Chatbot;

namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface ITokenStore
    {
        Task<ZaloTokens?> LoadAsync(CancellationToken ct = default);
        Task SaveAsync(ZaloTokens tokens, CancellationToken ct = default);
        bool IsExpired(ZaloTokens tokens, int skewSeconds = 60);
    }
}
