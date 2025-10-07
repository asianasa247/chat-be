using ManageEmployee.ViewModels.HotelModels;

namespace ManageEmployee.Services.Interfaces.Hotels;

public interface IGoodRoomPriceService
{
    Task Update(GoodRoomPriceModel form);

    Task<IEnumerable<GoodRoomPriceGetModel>> Get(GoodRoomPriceRequestModel form);
}
