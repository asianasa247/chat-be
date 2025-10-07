using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.Email
{
    public class MailLog : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
