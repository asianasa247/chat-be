using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatappLC.Application.DTOs.Messages
{
    public class SendMessageRequest
    {
        public string SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? GroupId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

}
