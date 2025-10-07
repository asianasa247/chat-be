namespace ManageEmployee.Entities.Chatbot
{
    // Lưu access token OA ở file JSON (không đụng DB)
    public class ZaloTokens
    {
        public string AccessToken { get; set; } = "";
        public string? RefreshToken { get; set; }
        public DateTimeOffset ExpiresAt { get; set; } // UTC
    }
}
