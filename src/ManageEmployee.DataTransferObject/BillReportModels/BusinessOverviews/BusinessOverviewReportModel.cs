namespace ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews
{
    public class BusinessOverviewReportCommonModel
    {
        public double RevenueAmount { get; set; }
        public double RefundAmount { get; set; }
        public double SaleAmount { get; set; }
        public double TotalCost { get; set; }
        public double GrossProfit { get; set; }
    }
    public class BusinessOverviewReportModel : BusinessOverviewReportCommonModel
    {
        public int TotalBill { get; set; }
        public int TotalBillPreviousPeriod { get; set; }
        public double RevenueAmountPreviousPeriod { get; set; }
        public double RefundAmountPreviousPeriod { get; set; }
        public double SaleAmountPreviousPeriod { get; set; }
        public double TotalCostPreviousPeriod { get; set; }
        public double GrossProfitPreviousPeriod { get; set; }
        public double RevenueAmountAverage { get; set; }
        public double RefundAmountAverage { get; set; }
        public double SaleAmountAverage { get; set; }
        public double TotalCostAverage { get; set; }
        public double GrossProfitAverage { get; set; }
        public int TotalBillAverage { get; set; }

    }
}
