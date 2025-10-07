using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IShiftService
{
    Task Create(ShiftModel form);
    Task Delete(int id);
    Task<ShiftModel> GetDetail(int id);
    Task<PagingResult<ShiftModel>> GetPaging(PagingRequestModel param);
    Task Update(ShiftModel form);
}
