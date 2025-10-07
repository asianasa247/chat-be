using ManageEmployee.Entities.UserEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class ReminderParticipant
    {
        public int ReminderId { get; set; }
        public ReminderModel Reminder { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
