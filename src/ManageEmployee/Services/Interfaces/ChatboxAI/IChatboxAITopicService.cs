using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChatboxAI;

namespace ManageEmployee.Services.Interfaces.ChatboxAI
{
    public interface IChatboxAITopicService
    {
        IEnumerable<ChatboxAITopic> GetAll();
        Task<PagingResult<ChatboxAITopic>> GetAll(int pageIndex, int pageSize, string keyword);
        Task<string> Create(ChatboxAITopic request);
        ChatboxAITopic GetById(int id);
        Task<string> Update(ChatboxAITopic request);
        string Delete(int id);
    }
}
