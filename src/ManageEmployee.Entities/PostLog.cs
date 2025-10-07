namespace ManageEmployee.Entities
{
    public class PostLog
    {
        public int Id { get; set; } // Khóa chính
        public string ImageUrl { get; set; } = ""; // ???ng d?n hình ?nh
        public string Status { get; set; } = ""; // Tr?ng thái bài vi?t
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Th?i gian t?o log
    }
}
