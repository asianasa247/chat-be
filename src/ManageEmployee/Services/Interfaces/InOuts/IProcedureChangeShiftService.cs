using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IProcedureChangeShiftService
{
    Task Accept(ProcedureChangeShiftModel form, int userId);
    Task<ProcedureCheckButton> CheckButton(int id, int userId);
    Task Create(ProcedureChangeShiftModel form, int userId);
    Task Delete(int id);
    Task<ProcedureChangeShiftModel> GetDetail(int id);
    Task<PagingResult<ProcedureChangeShiftModel>> GetPaging(ProcedureRequestOvertimePagingRequestModel param);
    Task<string> GetProcedureNumber();
    Task Update(ProcedureChangeShiftModel form, int userId);
}
