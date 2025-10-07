using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities.FAQ_AI_Chatbot
{
    public class FAQ_AIchat
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Department { get; set; }
        public string FirstTopic { get; set; }
    }
}
