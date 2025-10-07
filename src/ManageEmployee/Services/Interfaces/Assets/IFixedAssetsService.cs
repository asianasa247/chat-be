using ManageEmployee.Handlers;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

public interface IFixedAssetsService
{
    Task<FixedAsset> UpdateFromLedger(Ledger ledger, int year);

    Task<FixedAsset> GetById(int id);

    Task<List<FixedAssetsModelEdit>> GetListEdit(AssetsType eType, int iMonth, int iYear, int isInternal);

    Task<FixedAssetsModelEdit> UpdateEdit(FixedAssetsModelEdit entity, int year);

    string Delete(IEnumerable<int> ids);

    Task<PagingResult<FixedAssetViewModel>> SearchFixedAsset(PagingRequestModel searchRequest);

    string ExportExcel(PagingRequestModel searchRequest);

    string ImportExcel(List<FixedAssetViewModel> data);

    Task<CustomActionResult<List<FixedAssetsModelEdit>>> UpdateEditAccount(List<FixedAssetsModelEdit> entities, bool IsAutoAddDetail, int year);

    Task AddFixedAsset242FromFixedAsset(List<FixedAssetsModelEdit> entities, int year);
}
