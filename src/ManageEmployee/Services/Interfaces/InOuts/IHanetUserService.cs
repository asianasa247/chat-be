using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.InOuts
{
    public interface IHanetUserService
    {
        Task Delete(int id);
        Task<HanetUserModel> GetDetail(int id);
        Task<PagingResult<HanetUserModel>> GetPaging(PagingRequestModel param);
        Task Set(HanetUserModel form);
    }
}
