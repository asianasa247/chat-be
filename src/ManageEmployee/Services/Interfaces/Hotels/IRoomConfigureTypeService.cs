using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.ViewModels.HotelModels;

namespace ManageEmployee.Services.Interfaces.Hotels;

public interface IRoomConfigureTypeService
{
    Task<IEnumerable<RoomConfigureTypeModel>> GetList();

    Task<PagingResult<RoomConfigureTypeModel>> GetPaging(PagingRequestModel param);

    Task Create(RoomConfigureTypeModel form);

    Task Update(RoomConfigureTypeModel form);

    Task Delete(int id);

    Task<RoomConfigureTypeModel> GetById(int id);

    Task<IEnumerable<RoomConfigureTypeModel>> GetRoomConfigureForType(RoomConfigureTypeEnum type);
}
