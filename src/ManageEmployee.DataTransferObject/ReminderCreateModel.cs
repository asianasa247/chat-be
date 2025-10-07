using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject
{
    public class ReminderCreateModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Content { get; set; } = "";

        public ReminderRepeatType RepeatType { get; set; } = ReminderRepeatType.None;

        public int? DepartmentId { get; set; }

        // Dùng để hiển thị thông tin người nhận
        public List<ResponsiblePerson>? ParticipantPersons { get; set; }

        public bool IsAllUsers { get; set; } = false;
        public bool IsAllDepartment { get; set; } = false;
    }
}
