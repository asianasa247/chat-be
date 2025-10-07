using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Configs;

public interface IConfigDiscountService
{
    Task<PagingResult<ConfigDiscountModel>> GetPaging(PagingRequestModel form);

    Task Create(ConfigDiscountModel param);

    Task Update(ConfigDiscountModel param);

    Task Delete(int id);

    Task<ConfigDiscountModel> GetById(int id);
}
