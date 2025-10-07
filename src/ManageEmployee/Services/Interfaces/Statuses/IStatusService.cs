using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Services.Interfaces.Statuses;

public interface IStatusService
{
    Task<IEnumerable<Status>> GetAll(StatusTypeEnum type);

    Task<PagingResult<Status>> GetAll(StatusPagingRequest param);

    Task<Status> GetById(int id);


    Task<string> Create(Status param);

    Task<string> Update(Status param);

    Task<string> Delete(int id);
}
