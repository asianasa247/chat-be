using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IRequestExportGoodService
{
    Task Accept(int id, int userId);
    Task<ProcedureCheckButton> CheckButton(int id, int userId);
    Task Create(RequestExportGoodModel form, int userId);
    Task Delete(int id);
    Task<RequestExportGoodModel> GetDetail(int id);
    Task<PagingResult<RequestExportGoodPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
    Task<string> GetProcedureNumber();
    Task Update(RequestExportGoodModel form, int userId);
}
