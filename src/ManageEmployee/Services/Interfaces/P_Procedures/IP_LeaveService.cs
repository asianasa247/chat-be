using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IP_LeaveService
{
    Task<PagingResult<P_LeavePagingModel>> GetAll(ProcedurePagingRequestModel param, List<string> RoleName, int UserId);

    Task<P_LeaveViewModel> GetById(int id);

    Task Create(P_LeaveViewModel param, int userId);

    Task Update(P_LeaveViewModel param, int userId);

    Task Delete(int id);

    Task<string> GetProcedureNumber();

    Task Accept(P_LeaveViewModel param, int userId);
    Task NotAccept(int id, int userId);
}
