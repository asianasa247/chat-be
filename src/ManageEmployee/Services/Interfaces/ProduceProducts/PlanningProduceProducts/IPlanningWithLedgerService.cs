using ManageEmployee.DataTransferObject.ProduceProductModels;

namespace ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;

public interface IPlanningWithLedgerService
{
    Task SetDataAsync(PlanningProduceProductModel form, int userId);
}
