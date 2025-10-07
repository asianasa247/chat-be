using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.InOutEntities;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IInOutReportService
{
    Task<PagingResult<InOutReport>> GetPaging(PagingRequestModel param, int month, int yearFilter);
    Task SetData(int month, int yearFilter);
}
