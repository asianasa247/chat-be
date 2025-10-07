using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.Email
{
    public class MailTemplate:BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; } = "";
        public string Content { get; set; } = ""; //html content with parameter {param1}, {param2}, {param3}...
        public string Parameters { get; set; } //array join by comma (,)
        public bool Status { get; set; } = true;
    }
}
