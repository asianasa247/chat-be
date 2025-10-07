using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.Services.Interfaces.Goods
{
    public interface IGoodExporter
    {
        Task<string> GetExcelReport(SearchViewModel param, bool isManager);
    }
}
