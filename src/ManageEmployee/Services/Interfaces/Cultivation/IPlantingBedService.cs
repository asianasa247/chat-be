using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Cultivation;

namespace ManageEmployee.Services.Interfaces.Cultivation
{
    public interface IPlantingBedService
    {
        IEnumerable<PlantingBed> GetAll();
        Task<PagingResult<PlantingBed>> GetAll(int pageIndex, int pageSize, string keyword, int? regionId = null, int? typeId = null);
        Task<string> Create(PlantingBed request);
        PlantingBed GetById(int id);
        Task<string> Update(PlantingBed request);
        string Delete(int id);
    }
}
