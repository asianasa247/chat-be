using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;

namespace ManageEmployee.Services.Interfaces.ProduceProducts;

public interface IManufactureOrderService
{
    Task Accept(int id, int userId);

    Task Create(ManufactureOrderModel form, int userId);

    Task Delete(int id);

    Task<ManufactureOrderGetDetailModel> GetDetail(int id, int year);

    Task<IEnumerable<ManufactureOrderPagingModel>> GetList();

    Task<PagingResult<ManufactureOrderPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);

    Task NotAccept(int id, int userId);
    Task SetResponsibleUser(int id, int responsibleUserId, int userId);
    Task Update(ManufactureOrderModel form, int userId);

    Task UpdateForManufacture(ManufactureOrderGetDetailModel form, int userId);
    Task UpdateManufactureFromPaging(ManufactureOrderPagingModel form);
}
