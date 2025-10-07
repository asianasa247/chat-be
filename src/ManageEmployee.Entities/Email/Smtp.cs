using ManageEmployee.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.Email
{
    public class Smtp : BaseEntity
    {
        //Identity, Key
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SmtpServer { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public int Ssl { get; set; }
        public bool UseDefaultCredentials { get; set; } = true;
        public bool RequiresAuthentication { get; set; } = true;
        public bool EnableSsl { get; set; } = true;
        public bool EnableTls { get; set; } = true; //123
        public bool Status { get; set; } = true;//default true (active)
    }
}
