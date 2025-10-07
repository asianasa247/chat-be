using ManageEmployee.DataTransferObject.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ManageEmployee.DataTransferObject.Email
{
    public class MailSendModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int SmtpId { get; set; }//trong bảng Smtp
        public int ImapId { get; set; }//trong bảng Smtp, dùng để lấy email từ server
        public string DisplayName { get; set; }
        public bool Status { get; set; } = true;
    }
    public class MailInput
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string MailTo { get; set; }
        public string MailCc { get; set; }
        public string MailBcc { get; set; }
    }
    public class MailSendByTemplate
    {
        public string Email { get; set; }
        public int TemplateId { get; set; }
        public string MailTo { get; set; }//array join by comma (,)

    }
    public class MailSendSmtpModel : MailSendModel
    {
        public SmtpModel Smtp { get; set; }
    }
    public class MailBoxReq
    {
        public string Email { get; set; }
        public int ItemNo { get; set; }
        public int PageNo { get; set; }
    }
    public class MailBoxRes
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string TextContent { get; set; }
        public DateTime Date { get; set; }
        public uint Uid { get; set; }  // 👈 Thêm dòng này

    }
    public class MailDetailReq
    {
        public string Email { get; set; }
        public uint Uid { get; set; }
    }

    public class SendMailBody
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string MailTo { get; set; }
        public string MailCc { get; set; }
        public string MailBcc { get; set; }
    }
}
