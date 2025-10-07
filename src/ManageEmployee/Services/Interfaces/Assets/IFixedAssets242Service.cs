using ManageEmployee.Handlers;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

public interface IFixedAssets242Service
{
    Task<FixedAsset242> UpdateFromLedger(Ledger ledger, int year);
    Task<FixedAsset242> GetById(int id);
    Task<List<FixedAssetsModelEdit>> GetListEdit(AssetsType eType, int iMonth, int iYear, int isInternal);
    Task<string> Update(FixedAssetsModelEdit entity, int year);
    Task<CustomActionResult<List<FixedAssetsModelEdit>>> UpdateEdit(List<FixedAssetsModelEdit> entities, int isInternal, int year);
    string Deletes(IEnumerable<int> ids);

    PagingResult<FixedAssetExport> SearchFixedAsset(FixedAsset242RequestModel searchRequest);
    string ExportExcel(FixedAsset242RequestModel searchRequest);
    string ImportExcel(List<FixedAssetViewModel> data);
    string Delete(int id);
    Task<CustomActionResult<List<FixedAssetsModelEdit>>> UpdateEditAccount(List<FixedAssetsModelEdit> entities);
}
