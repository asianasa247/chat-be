using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountFilterService
{
    Task Create(ChartOfAccountFilter param);
    Task Update(ChartOfAccountFilter param);
    Task<List<ChartOfAccountFilter>> GetList();
    Task<PagingResult<ChartOfAccountFilter>> GetPaging(PagingRequestModel form);
    Task Delete(int id);
    Task<ChartOfAccountFilter> GetDetail(int id);
}
