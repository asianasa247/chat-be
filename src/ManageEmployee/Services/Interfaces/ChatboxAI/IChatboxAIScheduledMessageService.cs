using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChatboxAI;

namespace ManageEmployee.Services.Interfaces.ChatboxAI
{
    public interface IChatboxAIScheduledMessageService
    {
        IEnumerable<ChatboxAIScheduledMessage> GetAll();
        Task<PagingResult<ChatboxAIScheduledMessage>> GetAll(int pageIndex, int pageSize, string keyword, int? topicId = null);
        Task<string> Create(ChatboxAIScheduledMessage request);
        ChatboxAIScheduledMessage GetById(int id);
        Task<string> Update(ChatboxAIScheduledMessage request);
        string Delete(int id);
    }
}
