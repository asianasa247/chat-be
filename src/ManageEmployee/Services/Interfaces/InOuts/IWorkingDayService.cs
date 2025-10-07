using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.InOutEntities;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IWorkingDayService
{
    Task Create(WorkingDayModel form);
    Task Delete(int id);
    Task<WorkingDayModel> GetDetail(int id);
    Task<PagingResult<WorkingDay>> GetPaging(PagingRequestModel param);
    Task Update(WorkingDayModel form);
}
