namespace ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;

public interface IOrderProduceProductExcelService
{
    Task<string> ExportExcel(List<int> ids);
    Task ImportExcel(IFormFile file, int userId);
}
