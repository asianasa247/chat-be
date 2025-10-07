using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IRequestEquipmentOrderService
{
    Task Accept(int id, int userId);

    Task Create(int requestEquipmentId);

    Task Delete(int id);
    Task<string> Export(int requestEquipmentOrderId);
    Task<RequestEquipmentOrderModel> GetDetail(int id);
    Task<IEnumerable<SelectListModel>> GetList();
    Task<PagingResult<RequestEquipmentOrderPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);


    Task NotAccept(int id, int userId);

    Task Update(RequestEquipmentOrderModel form, int userId);
}
