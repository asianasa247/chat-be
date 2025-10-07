using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.Services.Interfaces.Ledgers.V2;

public interface IChartOfAccountV2Service
{
    Task<CommonModel> FindAccount(string code, string parentRef, int year);
}
