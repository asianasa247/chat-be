using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.Smtp
{
    public class SmtpModel
    {
        public int Id { get; set; }
        public string SmtpServer { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public int Ssl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool EnableSsl { get; set; }
        public bool EnableTls { get; set; }
        public bool RequiresAuthentication { get; set; }

    }
}
