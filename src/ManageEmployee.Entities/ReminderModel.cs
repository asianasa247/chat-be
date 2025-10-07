using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class ReminderModel
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Content { get; set; } = "";

        public ReminderRepeatType RepeatType { get; set; } = ReminderRepeatType.None;
        public ReminderStatus Status { get; set; } = ReminderStatus.Doing;

        public int? CreatedBy { get; set; }
        public int? DepartmentId { get; set; }

        public List<ReminderParticipant> Participants { get; set; } = new();

        public bool IsAllUsers { get; set; } = false;
        public bool IsAllDepartment { get; set; } = false;
    }
}
