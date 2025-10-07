using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.Contractors;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataLayer.Service.Interfaces;

public interface IContractorService
{
    Result<List<ContractorToCategoryDto>> GetCategoriesByContractorDomain(string domain);
    Result<List<ContractorToCategoryDto>> GetCategoriesByContractId(Guid contractId);
    Result<List<UserToContractorDto>> GetContractorsByUserId(int userId);
    Result<UserToContractorDto> GetContractorByUserId(int userId);
    Result<UserToContractorDto> GetContractorByDomain(string domain);
    Result<UserToContractorDto> AddContractor(UserToContractorDto dto);
    Result<ContractorToCategoryDto> AddCategoryToContractor(AddCategoryToContractorDto dto);
    Result<List<CategoryToProductsDto>> AddProductsToCategory(AddCategoryToProductsDto dto);
    Result<UserToContractorDto> UpdateContractor(UpdateUserToContractorDto dto);
    Result<List<Goods>> GetProductsByContractorCategoryId(Guid categoryId, int pageIndex, int pageSize);
    Result<List<Goods>> GetProductByContractDomain(string domain, int pageIndex, int pageSize);
}