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
    public class MailSend : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int SmtpId { get; set; }//trong bảng Smtp
        public int ImapId { get; set; }//trong bảng Smtp, dùng để lấy email từ server
        public string DisplayName { get; set; }
        public bool Status { get; set; } = true;
    }
}
