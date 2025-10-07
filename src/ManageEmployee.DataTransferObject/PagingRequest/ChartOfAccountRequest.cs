namespace ManageEmployee.DataTransferObject.PagingRequest;

public class ChartOfAccountRequest : PagingRequestModel
{
    public string? Account { get; set; }
    public string? Detail1 { get; set; }
    public string? Detail2 { get; set; }
}
