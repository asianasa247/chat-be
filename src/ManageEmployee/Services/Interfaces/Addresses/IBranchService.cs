using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.AddressEntities;

namespace ManageEmployee.Services.Interfaces.Addresses;

public interface IBranchService
{
    Task<IEnumerable<BranchModel>> GetAll();

    Task<PagingResult<BranchModel>> GetAll(int pageIndex, int pageSize, string keyword, int? type = null);

    Task<string> Create(Branch request);

    Task<BranchModel> GetById(int id);

    Task<string> Update(Branch request);

    Task<string> Delete(int id);
}
