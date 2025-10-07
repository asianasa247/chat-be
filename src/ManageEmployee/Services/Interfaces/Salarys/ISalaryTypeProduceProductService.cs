using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SalaryModels;

namespace ManageEmployee.Services.Interfaces.Salarys;

public interface ISalaryTypeProduceProductService
{
    Task Create(SalaryTypeProduceProductModel request, int userId);
    Task Delete(int id);
    Task<SalaryTypeProduceProductModel> GetById(int id);
    Task<PagingResult<SalaryTypeProduceProductPagingModel>> GetPaging(PagingRequestModel form);
    Task Update(SalaryTypeProduceProductModel request, int userId);
}
