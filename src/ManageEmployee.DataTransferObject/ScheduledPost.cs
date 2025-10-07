namespace ManageEmployee.DataTransferObject
{
    public class ScheduledPost
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public byte[] PhotoData { get; set; }
        public string PhotoFileName { get; set; }
        public string PhotoContentType { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string RepeatType { get; set; }
        public bool IsActive { get; set; } = true;
        public string PageId { get; set; }
        public string PageAccessToken { get; set; }
        public DateTime? PostedTime { get; set; } 
    }
}
