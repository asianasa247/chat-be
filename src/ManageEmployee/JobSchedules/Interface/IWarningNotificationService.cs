using ManageEmployee.Entities;

namespace ManageEmployee.JobSchedules.Interface;

public interface IWarningNotificationService
{
    Task<List<WarningNotification>> GetWarningNotification(int userId);
    Task WarningNotificationTrigger();
}
