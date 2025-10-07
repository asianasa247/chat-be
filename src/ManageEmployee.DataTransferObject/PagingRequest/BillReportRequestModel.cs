namespace ManageEmployee.DataTransferObject.PagingRequest;

public class BillReportRequestModel : PagingRequestModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    public string? Detail1 { get; set; }
    public int Type { get; set; } = 0;// customer = 0; good = 1; user = 2
    public string? AccountCode { get; set; }
}
