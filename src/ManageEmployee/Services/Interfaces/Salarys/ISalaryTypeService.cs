using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SalaryModels;

namespace ManageEmployee.Services.Interfaces.Salarys;

public interface ISalaryTypeService
{
    Task Create(SalaryTypeModel request);
    Task Delete(int id);
    Task<SalaryTypeModel> GetById(int id);
    Task<PagingResult<SalaryTypeModel>> GetPaging(PagingRequestModel form);
    Task Update(SalaryTypeModel request);
}
