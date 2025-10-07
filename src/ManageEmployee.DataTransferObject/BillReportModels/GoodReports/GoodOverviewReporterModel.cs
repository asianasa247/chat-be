
namespace ManageEmployee.DataTransferObject.BillReportModels.GoodReports
{

    public class GoodOverviewReporterModel
    {
        public GoodOverviewReporterOverviewModel Overview { get; set; }
        public IEnumerable<GoodOverviewReporterDetailModel> GroupGoods { get; set; }
        public IEnumerable<GoodOverviewReporterDetailModel> Goods { get; set; }
    }

    public class GoodOverviewReporterOverviewModel
    {
        public int TotalGood { get; set; }
        public int TotalQuantity { get; set; }
        public double SaleAmount { get; set; }
        public double GrossProfit { get; set; }
    }
    public class GoodOverviewReporterDetailModel : GoodOverviewReporterOverviewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double TotalRefund { get; set; }
    }
}
