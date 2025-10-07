using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public enum ReminderRepeatType
    {
        None = 0,
        OneDay = 1,
        TwoDays = 2,
        Daily = 3,
        Weekly = 4,
        Monthly = 5
    }

    public enum ReminderStatus
    {
        Doing = 0,
        Process = 1,  
        Complete = 2
    }
}
