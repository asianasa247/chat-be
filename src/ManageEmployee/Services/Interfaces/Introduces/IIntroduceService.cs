using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Introduces;

public interface IIntroduceService
{
    Task<IEnumerable<IntroduceModel>> GetAll();

    Task<PagingResult<IntroduceModel>> GetAll(PagingRequestTypeModel param);

    Task Create(IntroduceModel request);

    Task<IntroduceModel> GetById(int id);

    Task Update(IntroduceModel request);

    Task Delete(int id);
}
