namespace ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews
{
    public class BusinessReportForBranchModel : BusinessOverviewReportCommonModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
