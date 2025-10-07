namespace ManageEmployee.DataTransferObject.BillReportModels
{
    public class BillReportTimeRequestModel
    {
        public DateTime FromAt { get; set; }
        public DateTime ToAt { get; set; }
    }
    public class BillReportBranchRequestModel: BillReportTimeRequestModel
    {
        public int? BranchId { get; set; }
    }
    public class BillReportGoodRequestModel : BillReportTimeRequestModel
    {
        public string Account { get; set; }
        public string Detail1 { get; set; }
        public string Detail2 { get; set; }
    }
}
