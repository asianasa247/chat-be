namespace ManageEmployee.Entities.Chatbot
{
    public class CompanyInfo
    {
        public string Name { get; set; } = "";
        public string? Address { get; set; }
        public string? Hotline { get; set; }
        public string? Email { get; set; }
        public List<FaqItem> Faq { get; set; } = new();
    }

    public class FaqItem
    {
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
    }
}
