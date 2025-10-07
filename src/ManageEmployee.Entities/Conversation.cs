using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatHistory> ChatHistories { get; set; }
    }
}
