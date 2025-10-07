using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Cultivation;

namespace ManageEmployee.Services.Interfaces.Cultivation
{
    public interface IPlantingRegionService
    {
        IEnumerable<PlantingRegion> GetAll();
        Task<PagingResult<PlantingRegion>> GetAll(int pageIndex, int pageSize, string keyword, int? countryId = null, int? typeId = null);
        Task<string> Create(PlantingRegion request);
        PlantingRegion GetById(int id);
        Task<string> Update(PlantingRegion request);
        string Delete(int id);
    }
}
