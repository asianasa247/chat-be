namespace ManageEmployee.DataTransferObject.PagingRequest;

public class PagingRequestFilterDateModel : PagingRequestModel
{
    public DateTime? FromAt { get; set; }
    public DateTime? ToAt { get; set; }
}
