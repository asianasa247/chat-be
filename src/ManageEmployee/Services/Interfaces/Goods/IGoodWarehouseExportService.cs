using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodWarehouseExportService
{
    PagingResult<GoodWarehouseExportsViewModel> GetAll(GoodWarehouseExportRequestModel param);
    Task<string> Delete(int BillId);
}
