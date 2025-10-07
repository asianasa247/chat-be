using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class ContentMail
    {
        [Key]
        public int Id { get; set; } 
        public string? Title { get; set; }
        public string? BodyMail { get; set; }
        public int? Type { get; set; }
    }
}
