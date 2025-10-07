using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.ViewModels.HotelModels;

namespace ManageEmployee.Services.Interfaces.Hotels;

public interface IGoodRoomTypeService
{
    Task<PagingResult<GoodRoomTypePagingModel>> GetPaging(PagingRequestModel param);

    Task<GoodRoomTypeModel> GetById(int goodId);

    Task Update(GoodRoomTypeModel param);

    Task<IEnumerable<GoodRoomTypeSelectModel>> GetAll();

    Task<IEnumerable<GoodRoomTypeOrderModel>> GetRoomTypeForOrder(DateTime fromAt, DateTime toAt);
}
