using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.JobSchedules.Interface;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.JobSchedules;
public class WarningNotificationService: IWarningNotificationService
{
    private readonly ApplicationDbContext _dbContext;

    public WarningNotificationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task WarningNotificationTrigger()
    {
        var carFieldSetups = await _dbContext.CarFieldSetups.Where(x => x.WarningAt == DateTime.Today).ToListAsync();
        if (!carFieldSetups.Any())
            return;

        List<WarningNotification> warnings = new List<WarningNotification>();
        foreach (var carFieldSetup in carFieldSetups)
        {
            warnings.Add(new WarningNotification
            {
                Message = $"{carFieldSetup.Note}",
                Status = 0,
                WarningId = carFieldSetup.Id,
                //UserId = carFieldSetup.UserId,
                WarningTableName = typeof(CarFieldSetup).FullName,
                CreatedAt = DateTime.Now,
                Date = DateTime.Today,
            });
        }
        await _dbContext.WarningNotifications.AddRangeAsync(warnings);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<List<WarningNotification>> GetWarningNotification(int userId)
    {
        return await _dbContext.WarningNotifications.Where(x => x.UserId == userId && x.Date == DateTime.Today).ToListAsync();
    }
}
