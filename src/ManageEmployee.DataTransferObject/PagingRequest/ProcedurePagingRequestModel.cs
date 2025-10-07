using ManageEmployee.DataTransferObject.Enums;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class ProcedurePagingRequestModel : PagingRequestFilterDateModel
{
    public int? UserId { get; set; }
    public ProduceProductStatusTab StatusTab { get; set; }// 0: tất cả, 1: chờ duyệt, 2: đã duyệt, 3: hoàn thành
}
