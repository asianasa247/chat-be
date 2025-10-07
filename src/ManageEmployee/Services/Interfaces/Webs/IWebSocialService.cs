using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;

namespace ManageEmployee.Services.Interfaces.Webs;

public interface IWebSocialService
{
    Task<IEnumerable<SocialGetListModel>> GetAll();
    Task<PagingResult<SocialViewModel>> SearchNews(PagingRequestModel searchRequest);
    Task Create(SocialViewModel request);
    Task<SocialViewModel> GetById(int id);
    Task Update(SocialViewModel request);
    Task Delete(int id);
}
