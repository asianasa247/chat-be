namespace ManageEmployee.DataTransferObject.PagingRequest;

public class LedgerWarehousesRequestPaging : PagingRequestModel
{
    public int Month { get; set; }
    public int IsInternal { get; set; }
    public string? Type { get; set; }
}
