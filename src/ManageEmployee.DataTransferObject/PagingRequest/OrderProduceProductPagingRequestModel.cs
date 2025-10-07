using ManageEmployee.DataTransferObject.Enums;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class OrderProduceProductPagingRequestModel : PagingRequestFilterDateModel
{
    public ProduceProductStatusTab StatusTab { get; set; }// 0: tất cả, 1: chờ duyệt, 2: đã duyệt, 
}
