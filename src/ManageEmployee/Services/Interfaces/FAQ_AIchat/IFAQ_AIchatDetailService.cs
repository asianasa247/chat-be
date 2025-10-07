using ManageEmployee.DataTransferObject.FAQ_AIchat;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.FAQ_AIchat
{
    public interface IFAQ_AIchatDetailService
    {
        Task<IEnumerable<FAQ_AIchatDetailModel>> GetAll();
        Task<List<Entities.FAQ_AI_Chatbot.FAQ_AIchatDetail>> GetAllByChatID(int currentPage, int pageSize, int chatId);
        Task<PagingResult<FAQ_AIchatDetailModel>> GetAll(FAQ_AIchatDetailModel param);
        Task<Entities.FAQ_AI_Chatbot.FAQ_AIchatDetail> Create(FAQ_AIchatDetailModel request);
        Task<FAQ_AIchatDetailModel> GetById(int id);
        Task<Entities.FAQ_AI_Chatbot.FAQ_AIchatDetail> Update(FAQ_AIchatDetailModel request);
        Task<bool> Delete(int id);
        Task<List<FAQ_AIchatDetailModel>> GetAllbyChatId(int FaqAiChatId);

    }
}
