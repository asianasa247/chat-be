namespace ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews
{
    public class ReportForGroupGoodModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double AmountAverage { get; set; }
        public double AmountPreviousPeriod { get; set; }
        public int TotalBill { get; set; }
        public int TotalBillPreviousPeriod { get; set; }
        public int Id { get; set; }
    }

    public class ReportForCustomerModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double AmountAverage { get; set; }
        public double AmountPreviousPeriod { get; set; }
        public int TotalBill { get; set; }
        public int TotalBillPreviousPeriod { get; set; }
        public int CustomerId { get; set; }
    }

    public class ReportForUserModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double AmountAverage { get; set; }
        public double AmountPreviousPeriod { get; set; }
        public int TotalBill { get; set; }
        public int TotalBillPreviousPeriod { get; set; }
        public string UserCode { get; set; }
    }
    public class BusinessDetailForGroupGoodReporterModel
    {
        public IEnumerable<ReportForGroupGoodModel> GroupGoods { get; set; }
        public IEnumerable<ReportForGroupGoodModel> Goods { get; set; }
        public IEnumerable<ReportForCustomerModel> Customers { get; set; }
        public IEnumerable<ReportForUserModel> Users { get; set; }
    }
}