using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IP_SalaryAdvanceService
{
    Task<PagingResult<P_SalaryAdvancePagingViewModel>> GetAll(P_SalaryAdvanceRequestModel param, List<string> RoleName, int UserId);
    Task<string> Create(P_SalaryAdvanceViewModel request);
    P_SalaryAdvanceViewModel GetById(int id);
    Task<string> Update(P_SalaryAdvanceViewModel request);
    Task<string> Delete(int id);
    Task<string> GetProcedureNumber();
    Task<string> Accept(P_SalaryAdvanceViewModel request, int userId);
    Task AddLedger(int month, int year, int isInternal);
    Task<string> CreateForUser(P_SalaryAdvance_ItemForUser request, int userId);
    Task<PagingResult<P_SalaryAdvancePagingViewModelForUser>> GetAllForUser(PagingRequestModel param, int userId);
    Task<P_SalaryAdvance_ItemForUser> GetByIdForUser(int id);
    Task<string> UpdateForUser(P_SalaryAdvance_ItemForUser request);
    Task NotAccept(int id, int userId);
}
