using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IGatePassService
{
    Task Accept(int id, int userId);

    Task<ProcedureCheckButton> CheckButton(int id, int userId);

    Task Create(GatePassModel form, int userId);

    Task Delete(int id);

    Task<string> Export(int id);

    Task<GatePassModel> GetDetail(int id);

    Task<PagingResult<GatePassPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);

    Task<string> GetProcedureNumber();

    Task NotAccept(int id, int userId);

    Task Update(GatePassModel form, int userId);
}
