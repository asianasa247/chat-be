using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;

namespace ManageEmployee.Services.Interfaces.ProduceProducts;

public interface IProduceProductService
{
    Task Accept(int id, int userId);

    Task Create(int manufactureOrderId, int userId);

    Task Delete(int id);

    Task<ProduceProductModel> GetDetail(int id, int year);
    Task<IEnumerable<ProduceProductGetListModel>> GetList();
    Task<PagingResult<ProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);

    Task NotAccept(int id, int userId);
    Task SetLedgerExportProduct(int id, int year);
    Task SetLedgerImportProduct(int id, int year);
    Task Update(ProduceProductModel form, int userId);
}
