using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.Services.Interfaces.Goods
{
    public interface IGoodsInWarehouseReporter
    {
        Task<PagingResult<GoodsReportPositionModel>> ReportForGoodsInWarehouse(SearchViewModel param, int year);
    }
}
