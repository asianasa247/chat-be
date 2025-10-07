using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject
{
    public class ChatRequest
    {
        public string Prompt { get; set; }
        public Guid? ConversationId { get; set; }
        public IFormFile File { get; set; }
    }
}
