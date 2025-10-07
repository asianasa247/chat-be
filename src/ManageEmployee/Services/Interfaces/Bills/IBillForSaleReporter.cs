using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillForSaleReporter
{
    Task<PagingResult<BillDetailReport>> BaoCaoDoanhThuTheoNgay(int pageIndex, int pageSize, DateTime startDate, DateTime endDate);
    Task<string> BaoCaoDoanhThuTheoNgayExcel(SearchViewModel param);
    Task<PagingResult<BillDetailViewPaging>> BaoCaoLoiNhuanTruThue(int pageIndex, int pageSize, DateTime startDate, DateTime endDate, int type);
    Task<string> BaoCaoLoiNhuanTruThueExcel(SearchViewModel param);
}
