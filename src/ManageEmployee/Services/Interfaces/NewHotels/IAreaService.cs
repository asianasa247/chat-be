using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;

namespace ManageEmployee.Services.Interfaces.NewHotels
{
    public interface IAreaService
    {
            Task<Area> Add(AreaDTO model);
            Task<Area> Edit(int Id, AreaDTO model);
            Task<bool> Delete(int Id);
            Task<List<Area>> GetAll();
            Task<Area> GetById(int Id);
            Task<object> GetPaged(PagingRequestModel pagingRequest);
            Task<string> ExportExcel();
            Task<string> ImportExcel(IFormFile file);

    }
}
