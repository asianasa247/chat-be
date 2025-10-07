namespace ManageEmployee.DataTransferObject.PagingRequest;

public class DocumentType1RequestModel : PagingRequestModel
{
    public DateTime? FromAt { get; set; }
    public DateTime? ToAt { get; set; }
}
