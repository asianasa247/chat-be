namespace ManageEmployee.DataTransferObject.PagingRequest;

public class BillTrackingChangeStatusRequest
{
    public long Id { get; set; }
    public long BillId { get; set; }
    public string? CurrentTranType { get; set; }
}