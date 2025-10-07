namespace ManageEmployee.DataTransferObject.ChatbotAI
{
    public class ChatboxAIQAModel
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
    }
}
