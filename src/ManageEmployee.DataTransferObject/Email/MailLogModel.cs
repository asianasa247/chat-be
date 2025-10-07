using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.Email
{
    public class MailLogModel
    {
        public int Id { get; set; }
        public int MailTemplateId { get; set; }
        public string MailSend { get; set; } = "";
        public string MailTo { get; set; } = "";
        public string MailCc { get; set; } = "";
        public string MailBcc { get; set; } = "";
        public string Content { get; set; } = "";//Subject|Content
        public bool Success { get; set; } = true;
        public string Error { get; set; } = "";
        public bool Status { get; set; } = true;
    }
}
