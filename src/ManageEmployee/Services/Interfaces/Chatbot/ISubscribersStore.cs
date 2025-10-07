namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface ISubscribersStore
    {
        Task<HashSet<string>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(string userId, CancellationToken ct = default);
        Task RemoveAsync(string userId, CancellationToken ct = default);
    }
}
