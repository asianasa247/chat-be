using Hangfire;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.Departments;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IReminderService _reminderService;
        private readonly IFacebookMessengerService _fbMessengerService;
        private readonly IDepartmentService _departmentService;

        public ReminderController(
            IUserService userService,
            IReminderService reminderService,
            IFacebookMessengerService fbMessengerService,
            IDepartmentService departmentService)
        {
            _userService = userService;
            _reminderService = reminderService;
            _fbMessengerService = fbMessengerService;
            _departmentService = departmentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReminder([FromBody] ReminderCreateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Content))
                return BadRequest("Dữ liệu lời nhắc không hợp lệ.");

            if (model.StartTime <= DateTime.Now.AddMinutes(5))
                return BadRequest("Thời gian bắt đầu phải cách hiện tại ít nhất 5 phút.");
            var reminderResults = new List<object>();
            var repeatIntervals = GetRepeatIntervals(model.RepeatType);

            foreach (var offset in repeatIntervals)
            {
                var adjustedStartTime = model.StartTime.Add(offset);
                var adjustedEndTime = model.EndTime.Add(offset);

                var reminder = new ReminderModel
                {
                    CreatedAt = DateTime.Now,
                    Content = model.Content,
                    StartTime = adjustedStartTime,
                    EndTime = adjustedEndTime,
                    DepartmentId = model.DepartmentId,
                    RepeatType = model.RepeatType,
                    Status = ReminderStatus.Doing,
                    CreatedBy = null,
                    IsAllUsers = model.IsAllUsers,
                    IsAllDepartment = model.IsAllDepartment,
                    Participants = model.ParticipantPersons?.Select(p => new ReminderParticipant
                    {
                        UserId = p.UserId
                    }).ToList() ?? new List<ReminderParticipant>()
                };

                var reminderId = await _reminderService.CreateAsync(reminder);
                var users = await _userService.GetUsersByReminderAsync(reminder);
                var sentUsers = new List<object>();

                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.Note))
                    {
                        string contentMsg = $"📌 Nhắc nhở: {model.Content}\n🕑 Bắt đầu: {adjustedStartTime:HH:mm dd/MM/yyyy}\n⏳ Kết thúc: {adjustedEndTime:HH:mm dd/MM/yyyy}";
                        string lateMsg = $"Đã kết thúc: {model.Content}\n⏳ Kết thúc : {adjustedEndTime:HH:mm dd/MM/yyyy}";

                        var timeSendList = new List<string>();

                        for (int i = 3; i >= 1; i--)
                        {
                            var scheduledTime = adjustedStartTime.AddMinutes(-i);
                            if (scheduledTime > DateTime.Now)
                            {
                                var tempTime = scheduledTime;
                                BackgroundJob.Schedule(() =>
                                    _fbMessengerService.SendMessageAsync(user.Note, contentMsg), tempTime);

                                Console.WriteLine($"[Reminder] Gửi nhắc nhở tới {user.Note} lúc {tempTime}");
                                timeSendList.Add(tempTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }

                        if (adjustedEndTime > DateTime.Now)
                        {
                            BackgroundJob.Schedule(() =>
                                _fbMessengerService.SendButtonTemplateAsync(
                                    user.Note,
                                    lateMsg,
                                    new List<MessengerButton>
                                    {
                                        new MessengerButton("Tôi đã đọc", $"REMINDER_READ_{reminderId}_{user.Id}")
                                    }
                                ),
                                adjustedEndTime
                            );
                            Console.WriteLine($"[Reminder] Gửi nhắc trễ tới {user.Note} lúc {adjustedEndTime}");
                        }

                        sentUsers.Add(new
                        {
                            userId = user.Id,
                            userNote = user.Note,
                            timeSend = timeSendList,
                            timeLate = adjustedEndTime.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                }

                BackgroundJob.Schedule(() =>
                    _reminderService.UpdateStatusAsync(reminderId, ReminderStatus.Process),
                    adjustedStartTime);

                BackgroundJob.Schedule(() =>
                    _reminderService.UpdateStatusAsync(reminderId, ReminderStatus.Complete),
                    adjustedEndTime);

                reminderResults.Add(new
                {
                    reminderId,
                    startTime = adjustedStartTime,
                    endTime = adjustedEndTime,
                    sentTo = sentUsers
                });
            }

            return Ok(new
            {
                message = "Tạo lời nhắc thành công",
                reminders = reminderResults
            });
        }

        private List<TimeSpan> GetRepeatIntervals(ReminderRepeatType repeatType)
        {
            var intervals = new List<TimeSpan> { TimeSpan.Zero }; 

            switch (repeatType)
            {
                case ReminderRepeatType.OneDay:
                    intervals.Add(TimeSpan.FromDays(1));
                    break;

                case ReminderRepeatType.TwoDays:
                    intervals.Add(TimeSpan.FromDays(1)); 
                    intervals.Add(TimeSpan.FromDays(2)); 
                    break;

                case ReminderRepeatType.Daily:
                    for (int i = 1; i <= 6; i++) 
                        intervals.Add(TimeSpan.FromDays(i));
                    break;

                case ReminderRepeatType.Weekly:
                    intervals.Add(TimeSpan.FromDays(7));
                    break;

                case ReminderRepeatType.Monthly:
                    for (int i = 1; i <= 2; i++)
                        intervals.Add(TimeSpan.FromDays(30 * i));
                    break;
            }

            return intervals;
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users.Select(u => new
            {
                id = u.Id,
                username = u.Username,
                note = u.Note
            }));
        }


        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {
            var departments = _departmentService.GetAll();
            var result = departments.Select(d => new
            {
                d.Id,
                d.Name,
                d.Code,
                d.BranchId
            });

            return Ok(result);
        }

        [HttpPost("check-expired")]
        public async Task<IActionResult> CheckAndUpdateExpired()
        {
            var count = await _reminderService.CheckAndUpdateExpiredRemindersAsync();
            return Ok(new { message = $"Đã cập nhật {count} lời nhắc quá hạn." });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllReminders()
        {
            var reminders = await _reminderService.GetAllRemindersAsync();

            var result = reminders.Select(r => new
            {
                r.Id,
                r.Content,
                r.StartTime,
                r.EndTime,
                r.Status,
                r.RepeatType,
                r.CreatedAt,
                r.DepartmentId,
                r.IsAllUsers,
                r.IsAllDepartment,
                Participants = r.Participants.Select(p => new
                {
                    p.UserId,
                    UserName = p.User?.Username
                })
            });

            return Ok(result);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var success = await _reminderService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy lời nhắc để xóa." });

            return Ok(new { message = "Xóa lời nhắc thành công." });
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditReminder(int id, [FromBody] ReminderCreateModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Content))
                return BadRequest("Dữ liệu không hợp lệ.");

            if (model.StartTime <= DateTime.Now.AddMinutes(5))
                return BadRequest("Thời gian bắt đầu phải cách hiện tại ít nhất 5 phút.");

            var updatedReminder = new ReminderModel
            {
                Content = model.Content,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                DepartmentId = model.DepartmentId,
                RepeatType = model.RepeatType,
                IsAllUsers = model.IsAllUsers,
                IsAllDepartment = model.IsAllDepartment,
                Participants = model.ParticipantPersons?.Select(p => new ReminderParticipant
                {
                    UserId = p.UserId
                }).ToList() ?? new List<ReminderParticipant>()
            };

            var success = await _reminderService.UpdateAsync(id, updatedReminder);
            if (!success)
                return NotFound(new { message = "Không tìm thấy lời nhắc cần sửa." });

            return Ok(new { message = "Cập nhật lời nhắc thành công." });
        }

    }
}
