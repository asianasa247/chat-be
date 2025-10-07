using ManageEmployee.Entities.PocoSelections;

namespace ManageEmployee.DataTransferObject.BillReportModels.OperatingCosts
{
    public class OperatingCostOverviewReportModel
    {
        public IEnumerable<AccountBalanceSheetPocoData> Details { get; set; }
        public AccountBalanceSheetPocoData ItemTotal { get; set; }
    }
}