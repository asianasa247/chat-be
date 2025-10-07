namespace ManageEmployee.DataTransferObject.PagingRequest;

public class OrderProduceProductReportRequestModel : PagingRequestFilterDateModel
{
    public int Type { get; set; }// 0 = goods; 1= customer; 
}