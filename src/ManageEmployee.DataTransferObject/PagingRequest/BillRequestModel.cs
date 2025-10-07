namespace ManageEmployee.DataTransferObject.PagingRequest;

public class BillRequestModel : PagingRequestModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TypePay { get; set; }
    public int? CustomerId { get; set; }
    public int? GoodId { get; set; }
    public string? UserCode { get; set; }
    public string? Detail1 { get; set; }
}
