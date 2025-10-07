using ManageEmployee.DataTransferObject.InOutModels;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface IShiftUserService
{
    Task<ShiftUserModel> GetDetail(int userId, int month, int year);
    Task SyncUser(int id, int year);
    Task SetData(ShiftUserModel form, int year);
    Task SetShiftUserItem(int shiftUserId, ShiftUserDetailModel item);

    Task<bool> UpdateShiftUserItems(int shiftUserId, List<ShiftUserDetailModel> shiftUserDetailModels);
    Task<bool> UpdateShiftUsers(ShiftUserBodyRequestModel request);
    Task<List<ShiftUserFilterResponseModel>> FilterShiftUser(ShiftUserFilterModel request);
}
