using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.SupplyModels.VehicleRepairRequestModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IVehicleRepairRequestService
{
    Task Accept(int id, int userId);
    Task Create(VehicleRepairRequestModel form, int userId);
    Task Delete(int id);
    Task<string> Export(int vehicleRepairRequestId);
    Task<VehicleRepairRequestModel> GetDetail(int id, int userId);
    Task<IEnumerable<SelectListModel>> GetList();
    Task<PagingResult<VehicleRepairRequestPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
    Task<string> GetProcedureNumber();
    Task NotAccept(int id, int userId);
    Task Update(VehicleRepairRequestModel form, int userId);
}
