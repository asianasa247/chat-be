using ManageEmployee.DataTransferObject.Reports;

namespace ManageEmployee.Services.Interfaces.Accounts;

public interface IAccountBalanceSheetService
{
    Task<List<AccountBalanceItemModel>> GenerateReport(DateTime from, DateTime to, int year, string filterAccount = "", bool isNoiBo = false);
    Task<AccrualAccountingViewModel> GenerateAccrualAccounting(string type, DateTime? fromDate, DateTime? toDate, string accountCode, string detail1Code, string detail2Code, bool isNoiBo);
    AccountByPeriod GetAccountInfoByPeriod(DateTime from, DateTime to, string accountCode, int year, string detailOne = "", string detailTwo = "");
}
