namespace ManageEmployee.Services.Interfaces.ProduceProducts;

public interface IProduceProductLedgerService
{
    Task<bool> AddLedgerDebitFromOrderProduct(int orderProduceProductId, int year);
    Task AddLedgerDebitFromPlanningProduct(int planningProduceProductId, int carId, string carName, int year);
}
