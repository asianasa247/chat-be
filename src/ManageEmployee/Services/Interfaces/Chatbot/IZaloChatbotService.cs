namespace ManageEmployee.Services.Interfaces.Chatbot
{
    public interface IZaloChatbotService
    {
        /// <summary>
        /// Xây dựng câu trả lời theo flow: Chọn Chủ đề -> Chọn Câu hỏi -> Trả lời.
        /// Dùng IMemoryCache để giữ trạng thái theo userId (TTL ~30 phút).
        /// Fallback: nếu người dùng hỏi ngoài flow, trả lời nhanh dựa theo companyInfo.json.
        /// </summary>
        /// <param name="userId">Zalo user_id</param>
        /// <param name="userText">Tin nhắn người dùng gửi</param>
        /// <param name="ct">CancellationToken</param>
        Task<string> BuildReplyAsync(string userId, string userText, System.Threading.CancellationToken ct = default);
    }
}
