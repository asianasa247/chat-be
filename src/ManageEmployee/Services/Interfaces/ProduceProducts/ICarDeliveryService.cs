using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.ProduceProductEntities;

namespace ManageEmployee.Services.Interfaces.ProduceProducts;

public interface ICarDeliveryService
{
    Task Add(CarDelivery carDelivery, string tableName, int id);
    Task<CarDeliveryModel> Get(int? id);
    Task<List<CarDelivery>> GetListForTableName(string tableName, IEnumerable<int> tableIds);
    Task ResetTableId(CarDelivery carDelivery, int tableId);
    Task Set(CarDeliveryModel model, string tableName, int id);
}
