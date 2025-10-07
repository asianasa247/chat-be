using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces
{
    public interface IReminderService
    {
        Task<int> CreateAsync(ReminderModel reminder);
        Task SendStartReminderAsync(int reminderId);
        Task SendOverdueReminderAsync(int reminderId);
        Task UpdateStatusAsync(int reminderId, ReminderStatus newStatus);
        Task<int> CheckAndUpdateExpiredRemindersAsync();
        Task<List<ReminderModel>> GetAllRemindersAsync();
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, ReminderModel updatedReminder);

    }
}
