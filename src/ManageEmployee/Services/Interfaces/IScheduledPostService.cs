using System.Collections.Concurrent;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces
{
    public interface IScheduledPostService
    {
        Task AddScheduledPostAsync(ScheduledPost post);
        Task<List<ScheduledPost>> GetAllScheduledPostsAsync();
        Task<ScheduledPost> GetScheduledPostByIdAsync(Guid id);
        Task UpdateScheduledPostAsync(ScheduledPost post);
    }
}
