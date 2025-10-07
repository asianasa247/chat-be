using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountForCashierService
{
    Task<PagingResult<ChartOfAccountForCashier>> GetPagingChartOfAccountForCashier(ChartOfAccountForCashierPagingRequestModel param, int year);
}
