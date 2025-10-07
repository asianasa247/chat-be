using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;

namespace ManageEmployee.Services.Interfaces.NewHotels
{
    public interface IFloorService
    {
        Task<Floor> Add(FloorDTO model);
        Task<Floor> Update(int id, FloorDTO model);
        Task<bool> Delete(int id);
        Task<List<GetFloorModel>> GetAll();
        Task<GetFloorModel> GetById(int id);
        Task<object> GetPaged(PagingRequestModel pagingRequest);
    }

}
