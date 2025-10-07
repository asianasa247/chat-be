using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChatboxAI;

namespace ManageEmployee.Services.Interfaces.ChatboxAI
{
    public interface IChatboxAIQAService
    {
        IEnumerable<ChatboxAIQA> GetAll();
        Task<PagingResult<ChatboxAIQA>> GetAll(int pageIndex, int pageSize, string keyword, int? topicId = null);
        Task<string> Create(ChatboxAIQA request);
        ChatboxAIQA GetById(int id);
        Task<string> Update(ChatboxAIQA request);
        string Delete(int id);
    }
}
