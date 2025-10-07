using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IProcedureRequestOvertimeService
{
    Task Accept(int id, int userId);
    Task Copy(int id, List<int> userIds, int userIdSetter);
    Task Create(ProcedureRequestOvertimeModel form, int userId);
    Task Delete(int id);
    Task<ProcedureRequestOvertimeModel> GetDetail(int id);
    Task<PagingResult<ProcedureRequestOvertimePagingModel>> GetPaging(ProcedureRequestOvertimePagingRequestModel param, int userId);
    Task<string> GetProcedureNumber();
    Task NotAccept(int id, int userId);
    Task Update(ProcedureRequestOvertimeModel form, int userId);
}
