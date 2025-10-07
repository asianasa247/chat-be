using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Bills.BillReports.GoodInventories;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.GoodInventories
{
    public class GoodInventoryReporter : IGoodInventoryReporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ILedgerService _ledgerServices;

        public GoodInventoryReporter(ApplicationDbContext context,
            ILedgerService ledgerServices)
        {
            _context = context;
            _ledgerServices = ledgerServices;
        }

        public async Task<GoodInventoryOverviewModel> ReportAsync(int? branchId, int year)
        {
            var itemOut = new GoodInventoryOverviewModel();
            LedgerReportParamDetail form = new LedgerReportParamDetail()
            {
                AccountCode = "1561",
                FromDate = new DateTime(year, 1, 1),
                ToDate = DateTime.Today,
                IsNoiBo = true,
                FilterType = 2,
            };
            var datas = await _ledgerServices.GetDataReport_SoChiTiet_Six_data(form, year, wareHouseCode: null);

            itemOut.GoodQuantity = await _context.ChartOfAccounts.Where(x => x.Code == "1561").SumAsync(x => (x.OpeningStockQuantity ?? 0) + (x.ArisingStockQuantity ?? 0));
            itemOut.TotalQuantity = datas.Sum(x => x.CloseQuantity);
            itemOut.TotalAmount = itemOut.GoodQuantity * itemOut.TotalQuantity;
            itemOut.OutStockToday = datas.Sum(x => x.CloseQuantity);

            return itemOut;
        }
    }
}
public class GoodInventoryOverviewModel
{
    public double GoodQuantity { get; set; }
    public double TotalQuantity { get; set; }
    public double TotalAmount { get; set; }
    public double OutStockToday { get; set; }
    public double OutStockSevenday { get; set; }
    public double OutStockThrityday { get; set; }
    public double UnsoldQuantity { get; set; }
    public double UnsoldAmount { get; set; }
    public double SurpassingQuantity { get; set; }
    public double SurpassingAmount { get; set; }
}