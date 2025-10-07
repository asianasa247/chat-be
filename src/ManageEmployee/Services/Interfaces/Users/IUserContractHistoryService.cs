using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserContractHistoryService
{
    Task<PagingResult<UserContractHistoryModel>> GetPagingAsync(PagingRequestModel param);
    Task<UserContractHistory> GetDetail(int contractHistoryId);
}
