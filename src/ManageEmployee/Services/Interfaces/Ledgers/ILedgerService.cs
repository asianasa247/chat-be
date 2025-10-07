using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Handlers;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerService
{
    Task<PagingResultLedger<LedgerModel>> GetPage(LedgerRequestModel request, int year);

    Task<Ledger> Create(Ledger entity, int year);

    Task<Ledger> Update(Ledger currentLedger, Ledger entity, int year);

    Task<Ledger> GetLedgerById(long Id, int isInternal);

    Task<string> Delete(string idsStr, int isInternal, int year);

    Task<List<LedgerCostOfGoodsModel>> GetCostOfGoods(int iMonth, int iYear, int isInternal, int year);

    Task<string> CreateCostOfGoods(List<LedgerCostOfGoodsModel> entites, int isInternal, int year);

    Task<string> EditOrder(EditOrderRequestModel request, int year);

    Task<string> GetDataReport(LedgerReportParam _param, int year, bool isNoiBo = false);

    Task<double> DinhKhoanThue(LedgerRequestDinhKhoanThue request, int year);

    Task<List<LedgerPrint>> GetListDataPrint(string OrginalVoucherNumber, int isInternal, int year);

    Task<double> TinhGiaXuatKho(string code, string detail1, string detail2, string wareHouseCode, DateTime toDate,
        int year, int iMethodCalcExportPrice = 3, bool isInternal = false, List<SoChiTietViewModel> listXuatKho = null);

    Task<CustomActionResult<Ledger>> CreateFromFixedAsset(FixedAssetsModelEdit assets, AssetsType assetsType, int isInternal, int year);

    Task CreateFromFixedAsset242(FixedAssetsModelEdit assets, AssetsType assetsType, int year);

    Task<CustomActionResult<Ledger>> FindByFixedAssets(FixedAssetsModelEdit assets, int isInternal, int year);

    Task<int> FindByFixedAssetCheckMonths(FixedAssetsModelEdit assets, int year);

    void ValidateDataReport(LedgerReportParamDetail request);

    Task<List<LedgerReportTonSLViewModel>> GetDataReport_SoChiTiet_Six_data(LedgerReportParamDetail _param,
        int year, string wareHouseCode = "", List<SoChiTietViewModel> listXuatKho = null);
    Task<bool> XoaXuatKhoGiaVon();
}
