using ManageEmployee.DataTransferObject.FAQ_AIchat;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.FAQ_AI_Chatbot;
using ManageEmployee.ViewModels;

namespace ManageEmployee.Services.Interfaces.FAQ_AIchat
{
    public interface IFAQ_AIchatService
    {
        Task<IEnumerable<FAQ_AIchatModel>> GetAll();
        Task<List<Entities.FAQ_AI_Chatbot.FAQ_AIchat>> GetAll(int currentPage, int pageSize);
        Task<PagingResult<FAQ_AIchatModel>> GetAll(FAQ_AIchatModel param);
        Task<Entities.FAQ_AI_Chatbot.FAQ_AIchat> Create(FAQ_AIchatModel request);
        Task<FAQ_AIchatModel> GetById(int id);
        Task<Entities.FAQ_AI_Chatbot.FAQ_AIchat> Update(FAQ_AIchatModel request);
        Task<bool> Delete(int id);
        Task<int> TotalChat();
    }
}
