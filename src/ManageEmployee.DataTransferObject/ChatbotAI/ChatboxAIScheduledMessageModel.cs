namespace ManageEmployee.DataTransferObject.ChatbotAI
{
    public class ChatboxAIScheduledMessageModel
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public string Message { get; set; } = "";
        public TimeSpan SendTime { get; set; }
        public string? DaysOfWeek { get; set; }
        public DateTime? LastSentAt { get; set; }
    }
}
