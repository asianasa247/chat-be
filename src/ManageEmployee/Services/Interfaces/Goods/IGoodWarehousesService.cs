using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodWarehousesService
{
    Task<PagingResult<GoodWarehousesViewModel>> GetAll(SearchViewModel param);
    Task<string> SyncChartOfAccount(int year);
    Task<string> SyncTonKho(int year);
    Task<object> CompleteBill(List<BillDetailViewPaging> billDetails, bool isForce, int userId);
    Task Update(GoodWarehousesUpdateModel item, double quantityChange = 0);
    Task<GoodWarehousesUpdateModel> GetById(int id);
    Task<string> UpdatePrintedStatus(int[] ids);
    Task<ReportForBranchModel> ReportWareHouse(int warehouseId, int shelveId, int floorId, string type);
    Task Create(Ledger entity, int year);
}
