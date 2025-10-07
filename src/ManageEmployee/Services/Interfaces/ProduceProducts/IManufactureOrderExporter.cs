
namespace ManageEmployee.Services.Interfaces.ProduceProducts
{
    public interface IManufactureOrderExporter
    {
        Task<string> ExportGoodDetail(int id);
        Task<string> ExportPdf(int id);
    }
}
