using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.ViewModels;
using System.Linq.Expressions;
using GoodsEntity = ManageEmployee.Entities.GoodsEntities.Goods;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodsService
{
    Task<IEnumerable<GoodsEntity>> GetAll(Expression<Func<GoodsEntity, bool>> where, int pageSize = 10);

    Task<GoodsPagingResult> GetPaging(SearchViewModel param, int year);

    Task<GoodslDetailModel> GetById(int id, int year);

    Task<IEnumerable<SelectListModel>> GetAllGoodShowWeb();

    Task<byte[]> ExportExcelSCT(int year); //sổ chi tiết
    Task<byte[]> ExportExcelCVP(int year); //convertproduct
    Task<List<InventoryProductStockViewModel>> GetConvertProductStockData(int year);
    Task<List<InventoryProductStockViewModel>> GetConvertProductStockDataFilter(int year, DateTime? fromDate, DateTime? toDate, string warehouse, string productName);
    Task<PagingResult<InventoryProductStockViewModel>> GetConvertProductStockData(int year, int page, int pageSize);
    Task<InventoryStockResponse> GetConvertProductStockDataPaginator(int year, int page, int pageSize);
    IQueryable<GoodsExportlModel> GetAll_Common(SearchViewModel param);
}