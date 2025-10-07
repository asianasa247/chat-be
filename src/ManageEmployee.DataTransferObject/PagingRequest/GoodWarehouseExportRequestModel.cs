namespace ManageEmployee.DataTransferObject.PagingRequest;

public class GoodWarehouseExportRequestModel : PagingRequestModel
{
    public DateTime? Fromdt { get; set; }
    public DateTime? Todt { get; set; }
}