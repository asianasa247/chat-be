using Hangfire;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessengerServices _messengerService;
        private readonly IFacebookMessengerService _facebookMessengerService;
        public ReminderService(ApplicationDbContext context, IMessengerServices messengerService, IFacebookMessengerService facebookMessengerService)
        {
            _context = context;
            _messengerService = messengerService;
            _facebookMessengerService = facebookMessengerService;
        }

        public async Task<int> CreateAsync(ReminderModel model)
        {
            _context.Reminders.Add(model);
            await _context.SaveChangesAsync();

            var remindBeforeStart = model.StartTime.AddMinutes(-1);
            if (remindBeforeStart > DateTime.Now)
            {
                BackgroundJob.Schedule(() => SendStartReminderAsync(model.Id), remindBeforeStart);
            }

            if (model.EndTime > DateTime.Now)
            {
                BackgroundJob.Schedule(() => SendOverdueReminderAsync(model.Id), model.EndTime);
            }

            return model.Id;
        }

        public async Task SendStartReminderAsync(int reminderId)
        {
            var model = await _context.Reminders.FindAsync(reminderId);
            if (model == null) return;

            var users = await GetTargetUsers(model);
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Note))
                {
                    await _messengerService.SendMessageAsync(user.Note,
                        $"🔔 Nhắc nhở: {model.Content}\n🕒 Bắt đầu: {model.StartTime:HH:mm dd/MM/yyyy}\n⏰ Kết thúc: {model.EndTime:HH:mm dd/MM/yyyy}");
                }
            }
        }

        public async Task SendOverdueReminderAsync(int reminderId)
        {
            var model = await _context.Reminders.FindAsync(reminderId);
            if (model == null || model.Status != ReminderStatus.Doing) return;

            var users = await GetTargetUsers(model);
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Note))
                {
                    await _messengerService.SendMessageAsync(user.Note,
                        $"❗ Bạn đã trễ hạn lời nhắc: {model.Content}\n⏰ Hạn cuối: {model.EndTime:HH:mm dd/MM/yyyy}");
                }
            }
        }

        private async Task<List<UserModel>> GetTargetUsers(ReminderModel model)
        {
            var query = _context.Users.AsQueryable();
            var result = new List<UserModel>();

            // 1. Toàn bộ người dùng
            if (model.IsAllUsers)
            {
                return await query.Select(u => new UserModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Note = u.Note
                }).ToListAsync();
            }

            // 2. Người dùng theo Participants
            if (model.Participants?.Any() == true)
            {
                var ids = model.Participants.Select(p => p.UserId).Distinct().ToList();
                var users = await query
                    .Where(u => ids.Contains(u.Id))
                    .Select(u => new UserModel
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Note = u.Note
                    }).ToListAsync();
                result.AddRange(users);
            }

            // 3. Người dùng trong phòng ban
            if (model.IsAllDepartment && model.DepartmentId.HasValue)
            {
                var users = await query
                    .Where(u => u.DepartmentId == model.DepartmentId.Value)
                    .Select(u => new UserModel
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Note = u.Note
                    }).ToListAsync();
                result.AddRange(users);
            }

            // 4. Trả về danh sách không trùng lặp
            return result
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();
        }
        public async Task UpdateStatusAsync(int reminderId, ReminderStatus newStatus)
        {
            var reminder = await _context.Reminders.FindAsync(reminderId);
            if (reminder == null) return;

            reminder.Status = newStatus;
            _context.Reminders.Update(reminder);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CheckAndUpdateExpiredRemindersAsync()
        {
            var now = DateTime.Now;

            var reminders = await _context.Reminders
                .Where(r => r.Status != ReminderStatus.Complete)
                .ToListAsync();

            foreach (var reminder in reminders)
            {
                if (now < reminder.StartTime)
                {
                    reminder.Status = ReminderStatus.Doing;
                }
                else if (now >= reminder.StartTime && now <= reminder.EndTime)
                {
                    reminder.Status = ReminderStatus.Process;
                }
                else // now > reminder.EndTime
                {
                    reminder.Status = ReminderStatus.Complete;
                }
            }

            if (reminders.Any())
            {
                _context.Reminders.UpdateRange(reminders);
                await _context.SaveChangesAsync();
            }

            return reminders.Count;
        }
        public async Task<List<ReminderModel>> GetAllRemindersAsync()
        {
            return await _context.Reminders
                .Include(r => r.Participants)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var reminder = await _context.Reminders
                .Include(r => r.Participants)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reminder == null)
                return false;

            //_context.ReminderParticipants.RemoveRange(reminder.Participants);
            _context.Reminders.Remove(reminder);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateAsync(int id, ReminderModel updatedReminder)
        {
            var existing = await _context.Reminders
                .Include(r => r.Participants)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existing == null)
                return false;

            existing.Content = updatedReminder.Content;
            existing.StartTime = updatedReminder.StartTime;
            existing.EndTime = updatedReminder.EndTime;
            existing.DepartmentId = updatedReminder.DepartmentId;
            existing.RepeatType = updatedReminder.RepeatType;
            existing.IsAllUsers = updatedReminder.IsAllUsers;
            existing.IsAllDepartment = updatedReminder.IsAllDepartment;

            //_context.ReminderParticipants.RemoveRange(existing.Participants);
            existing.Participants = updatedReminder.Participants;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
