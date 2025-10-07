using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.FAQ_AI_Chatbot
{
    public class FAQ_AIchatDetail
    {
        public int Id { get; set; }
        public int FAQ_AIchatId { get; set; }
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Topic { get; set; }
    }
}
