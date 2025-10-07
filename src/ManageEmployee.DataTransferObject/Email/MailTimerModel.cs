using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.Email
{
    public class MailTimerModel
    {
        public int Id { get; set; }
        public int MailTemplateId { get; set; }
        public string EmailSend { get; set; } = "";
        public string MailTo { get; set; } = "";
        public string MailCc { get; set; } = "";
        public string MailBcc { get; set; } = "";
        public string Content { get; set; } = "";
        public int TypeRepeat { get; set; } = 0; //0: one time, 1: daily, 2: weekly, 3: monthly
        public int RepeatInterval { get; set; } = 0; //0: one time, 1: every 1 day, 2: every 2 days, 3: every 3 days, 4: every 4 days, 5: every 5 days, 6: every 6 days, 7: every 7 days
        public TimeSpan Begin { get; set; } = new TimeSpan(12, 0, 0); //default 13:00:00 or 1:00 PM
        public bool IsRunning { get; set; } = true; //default true (running)
        public bool Status { get; set; } = true; //default true (active)
        public DateTime LastSent { get; set; } = DateTime.Now;
    }
    public class MailTimerTemplate : MailTimerModel
    {
        public MailTemplateModel MailTemplate { get; set; }
    }
}
