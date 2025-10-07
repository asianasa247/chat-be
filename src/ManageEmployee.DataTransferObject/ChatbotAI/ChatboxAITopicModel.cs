using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.ChatbotAI
{
    public class ChatboxAITopicModel
    {
        public int Id { get; set; }
        public string TopicCode { get; set; } = "";

        public string TopicName { get; set; } = "";
    }
}
