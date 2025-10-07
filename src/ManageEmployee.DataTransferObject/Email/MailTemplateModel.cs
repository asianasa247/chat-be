using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.Email
{
    public class MailTemplateModel
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; } = "";
        public string Content { get; set; } = ""; //html content with parameter {param1}, {param2}, {param3}...
        public string Parameters { get; set; } //array join by comma (,)
        public bool Status { get; set; } = true;
    }
}
