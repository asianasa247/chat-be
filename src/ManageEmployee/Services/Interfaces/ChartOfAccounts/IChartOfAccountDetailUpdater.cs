using ManageEmployee.DataTransferObject.ChartOfAccountModels;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountDetailUpdater
{
    Task<string> UpdateAccountDetail(ChartOfAccountModel model, int year);
}
