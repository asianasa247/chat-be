using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Events;

public interface IEventWithImageService
{
    Task Create(EventWithImageModel param);

    Task Delete(int id);

    Task<PagingResult<EventWithImagePagingGetterModel>> GetAll(PagingRequestModel param);

    Task<EventWithImageDetailGetterModel> GetById(int id);

    Task Update(EventWithImageModel param);
}
