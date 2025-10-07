namespace ManageEmployee.Services.Interfaces.Bills.BillReports.GoodInventories
{
    public interface IGoodInventoryReporter
    {
        Task<GoodInventoryOverviewModel> ReportAsync(int? branchId, int year);
    }
}
