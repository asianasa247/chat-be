namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface ISubscribersStore
    {
        // NEW: đa-app theo appCode
        Task<HashSet<string>> GetAllAsync(string appCode, CancellationToken ct = default);
        Task AddAsync(string appCode, string userId, CancellationToken ct = default);
        Task RemoveAsync(string appCode, string userId, CancellationToken ct = default);
    }
}
