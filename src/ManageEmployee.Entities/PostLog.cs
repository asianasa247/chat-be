namespace ManageEmployee.Entities
{
    public class PostLog
    {
        public int Id { get; set; } // Kh�a ch�nh
        public string ImageUrl { get; set; } = ""; // ???ng d?n h�nh ?nh
        public string Status { get; set; } = ""; // Tr?ng th�i b�i vi?t
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Th?i gian t?o log
    }
}
