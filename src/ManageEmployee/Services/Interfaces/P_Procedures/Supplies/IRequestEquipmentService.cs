using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IRequestEquipmentService
{
    Task Accept(int id, int userId);
    Task Create(RequestEquipmentModel form, int userId);
    Task Delete(int id);
    Task<string> Export(int requestEquipmentId);
    Task<RequestEquipmentModel> GetDetail(int id, int userId);
    Task<IEnumerable<SelectListModel>> GetList();
    Task<PagingResult<RequestEquipmentPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
    Task<string> GetProcedureNumber();
    Task NotAccept(int id, int userId);
    Task Update(RequestEquipmentModel form, int userId);
}
