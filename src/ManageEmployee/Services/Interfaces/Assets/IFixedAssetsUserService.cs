using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DocumentEntities;

namespace ManageEmployee.Services.Interfaces.Assets;

public interface IFixedAssetsUserService
{
    Task<FixedAssetUser> GetById(int id);
    IQueryable<FixedAssetViewModel> GetListEdit(PagingRequestModel searchRequest);
    Task<FixedAssetUser> UpdateEdit(FixedAssetUser entity);
    string Delete(int id);
    Task<string> ExportExcel(PagingRequestModel searchRequest);
    string ImportExcel(List<FixedAssetViewModel> datas);
    Task<FixedAssetUser> Create(FixedAssetUser entity, int year);
    Task<FixedAssetUserGetterModel> GetByIdV2(int id, int year);
}
