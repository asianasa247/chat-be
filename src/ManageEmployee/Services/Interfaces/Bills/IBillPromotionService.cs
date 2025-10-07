using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Entities.BillEntities;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillPromotionService
{
    Task Copy(List<BillPromotion> billPromotionSources, int tableId, string tableName);
    Task Create(List<BillPromotionModel> billPromotionRequests, int tableId, string tableName, int? carId = null, string carName = null, int? customerId = null);
    Task<List<BillPromotionModel>> Get(int tableId, string tableName, int? carId = null, string carName = null, int? customerId = null);
    Task<List<BillPromotion>> GetBillPromotion(int tableId, string tableName, int? carId = null, string carName = null, int? customerId = null);
    Task<double> GetTotalAmountPromotion(int tableId, string tableName);
}
