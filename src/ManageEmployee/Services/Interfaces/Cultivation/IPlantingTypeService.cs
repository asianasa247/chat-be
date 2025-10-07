using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Cultivation;

namespace ManageEmployee.Services.Interfaces.Cultivation
{
    public interface IPlantingTypeService
    {
        IEnumerable<PlantingType> GetAll();
        Task<PagingResult<PlantingType>> GetAll(int pageIndex, int pageSize, string keyword, int? category = null);
        Task<string> Create(PlantingType request);
        PlantingType GetById(int id);
        Task<string> Update(PlantingType request);
        string Delete(int id);
    }
}
