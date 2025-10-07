using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class ChatHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Prompt { get; set; }
        public string Response { get; set; }
        public bool IsTrainedContent { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
