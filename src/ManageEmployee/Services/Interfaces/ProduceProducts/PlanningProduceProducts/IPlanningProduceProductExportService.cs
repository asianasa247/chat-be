namespace ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;

public interface IPlanningProduceProductExportService
{
    Task<List<string>> ExportForCar(int? carId, string carCode, int id);
    Task<string> ExportFull(int id);

    Task<string> ExportGatePass(int? carId, string carCode, int id);

    Task<string> ExportPaymentProposal(int? carId, string carName, int id);
}
